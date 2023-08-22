using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Registration;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.Registration;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.RegistrationTests;

[TestFixture]
public class TeraAgentRegistrationServiceTests
{
    private const int TestNewTeraAgentId = 1;
    private const int TestExistingTeraAgentId = 132;
    private const int TestNanoInstanceId = 331;
    private const int TestTeraPlatformId = 221;
    private static readonly Guid TestTeraId = new("{4AA3E5B5-32E6-42B4-91CA-CD05D4DA15B3}");

    [Test]
    public async Task Register_WhenProvidedWithNoTeraId_GeneratesANewTeraId()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var parameters = new TeraAgentRegistrationParameters
        {
            NanoInstanceId = new SplinterId(),
            TeraAgentStatus = TeraAgentStatus.Migrating
        };

        AddDefaultData(dbContext);

        var teraId = await service.Register(TestTeraPlatformId, parameters);
        var teraAgent = dbContext.TeraAgents.Single();

        teraId.Should().Be(teraAgent.TeraId);
        teraAgent.Id.Should().Be(TestNewTeraAgentId);
        teraAgent.NanoInstanceId.Should().Be(TestNanoInstanceId);
        teraAgent.TeraPlatformId.Should().Be(TestTeraPlatformId);
        teraAgent.Status.Should().Be(TeraAgentStatus.Migrating);
    }

    [Test]
    public async Task Register_WhenProvidedWithATeraIdAndTeraAgentDoesNotExist_GeneratesATeraAgentWithTheSpecifiedTeraId()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var parameters = new TeraAgentRegistrationParameters
        {
            NanoInstanceId = new SplinterId(),
            TeraAgentStatus = TeraAgentStatus.Disposed,
            TeraId = TestTeraId
        };

        AddDefaultData(dbContext);

        var teraId = await service.Register(TestTeraPlatformId, parameters);
        var teraAgent = dbContext.TeraAgents.Single();

        teraId.Should().Be(TestTeraId);
        teraAgent.Id.Should().Be(TestNewTeraAgentId);
        teraAgent.TeraId.Should().Be(TestTeraId);
        teraAgent.NanoInstanceId.Should().Be(TestNanoInstanceId);
        teraAgent.TeraPlatformId.Should().Be(TestTeraPlatformId);
        teraAgent.Status.Should().Be(TeraAgentStatus.Disposed);
    }

    [Test]
    public async Task Register_WhenProvidedWithATeraIdAndTeraAgentExists_UpdatesTheExistingTeraAgent()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var parameters = new TeraAgentRegistrationParameters
        {
            NanoInstanceId = new SplinterId(),
            TeraAgentStatus = TeraAgentStatus.Disposed,
            TeraId = TestTeraId
        };

        AddDefaultData(dbContext);
        AddExistingTeraAgent(dbContext);

        var teraId = await service.Register(TestTeraPlatformId, parameters);
        var teraAgent = dbContext.TeraAgents.Single();

        teraId.Should().Be(TestTeraId);
        teraAgent.Id.Should().Be(TestExistingTeraAgentId);
        teraAgent.TeraId.Should().Be(TestTeraId);
        teraAgent.NanoInstanceId.Should().Be(TestNanoInstanceId);
        teraAgent.TeraPlatformId.Should().Be(TestTeraPlatformId);
        teraAgent.Status.Should().Be(TeraAgentStatus.Disposed);
    }

    private static void AddDefaultData(TeraDbContext teraDbContext)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);

        builder.AddTeraPlatform(TestTeraPlatformId);
        builder.AddNanoInstance(TestNanoInstanceId);
    }

    private static void AddExistingTeraAgent(TeraDbContext teraDbContext)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);

        builder.AddTeraAgent(
            TestTeraPlatformId,
            TestNanoInstanceId,
            TestExistingTeraAgentId,
            TestTeraId);
    }

    private static INanoTypeManager GetDefaultNanoTypeManager()
    {
        var result = new Mock<INanoTypeManager>();

        result
            .Setup(m => m.GetNanoInstanceId(It.IsAny<SplinterId>()))
            .ReturnsAsync(TestNanoInstanceId);

        return result.Object;
    }

    private static ITeraAgentRegistrationService GetService(
        TeraDbContext teraDbContext)
    {
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>().Object;
        var mockTeraAgentManager = new Mock<ITeraAgentManager>().Object;

        return new TeraAgentRegistrationService(
            teraDbContext, GetDefaultNanoTypeManager(), mockTeraAgentManager, mockDependencyService);
    }
}