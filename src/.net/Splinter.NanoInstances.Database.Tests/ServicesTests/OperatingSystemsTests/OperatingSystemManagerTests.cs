using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.OperatingSystems;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Platforms;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.OperatingSystemsTests;

[TestFixture]
public class OperatingSystemManagerTests
{
    private const int TestNewOperatingSystemId = 1;
    private const int TestExistingOperatingSystemId = 341;
    private const string TestOsDescription = "Windows Test";
    private const ProcessorArchitecture TestProcessorArchitecture = ProcessorArchitecture.x86_64;
    private const OperatingSystem TestOperatingSystem = OperatingSystem.Windows;

    [Test]
    public async Task RegisterOperatingSystemInformation_WhenOperatingSystemInfoDoesNotExist_CreatesANewEntry()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var manager = GetOperatingSystemManager(dbContext);

        var id = await manager.RegisterOperatingSystemInformation();
        var os = dbContext.OperatingSystems.Single();

        id.Should().Be(TestNewOperatingSystemId);
        os.Id.Should().Be(TestNewOperatingSystemId);
        os.Description.Should().Be(TestOsDescription);
        os.ProcessorArchitecture.Should().Be(TestProcessorArchitecture);
        os.Type.Should().Be(TestOperatingSystem);
    }

    [Test]
    public async Task RegisterOperatingSystemInformation_WhenOperatingSystemInfoExists_ReturnsExistingId()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var manager = GetOperatingSystemManager(dbContext);

        AddExistingOperatingSystem(dbContext);

        var id = await manager.RegisterOperatingSystemInformation();
        var os = dbContext.OperatingSystems.Single();

        id.Should().Be(TestExistingOperatingSystemId);
        os.Id.Should().Be(TestExistingOperatingSystemId);
        os.Description.Should().Be(TestOsDescription);
        os.ProcessorArchitecture.Should().Be(TestProcessorArchitecture);
        os.Type.Should().Be(TestOperatingSystem);
    }

    private static void AddExistingOperatingSystem(TeraDbContext teraDbContext)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);

        builder.AddOperatingSystem(
            TestExistingOperatingSystemId,
            TestOsDescription,
            TestProcessorArchitecture,
            TestOperatingSystem);
    }

    private static IOperatingSystemInformationProvider GetDefaultOsProvider()
    {
        var result = new Mock<IOperatingSystemInformationProvider>();

        result
            .Setup(p => p.GetOperatingSystemInformation())
            .ReturnsAsync(new OperatingSystemInformation
            {
                Description = TestOsDescription,
                ProcessorArchitecture = TestProcessorArchitecture,
                Type = TestOperatingSystem
            });

        return result.Object;
    }

    private static IOperatingSystemManager GetOperatingSystemManager(TeraDbContext teraDbContext)
    {
        return new OperatingSystemManager(teraDbContext, GetDefaultOsProvider());
    }
}