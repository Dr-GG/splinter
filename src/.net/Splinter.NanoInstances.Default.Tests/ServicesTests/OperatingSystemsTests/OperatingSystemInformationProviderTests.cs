using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.OperatingSystemsTests;

public class OperatingSystemInformationProviderTests
{
    [Test]
    public async Task GetFrameworkDescription_WhenInvoked_ReturnsAString()
    {
        var provider = GetDefaultProvider();
        var description = await provider.GetFrameworkDescription();

        description.Should().NotBeNullOrEmpty();
        description.Should().Contain(".NET");
    }

    [Test]
    public async Task GetOperatingSystemInformation_WhenInvoked_ReturnsData()
    {
        var provider = GetDefaultProvider();
        var data = await provider.GetOperatingSystemInformation();

        data.Should().NotBeNull();
        data.Type.Should().BeOneOf(OperatingSystem.Linux, OperatingSystem.Windows);
        data.ProcessorArchitecture.Should().BeDefined();
        data.Description.Should().NotBeNullOrEmpty();
    }

    private static IOperatingSystemInformationProvider GetDefaultProvider()
    {
        return new OperatingSystemInformationProvider();
    }
}
