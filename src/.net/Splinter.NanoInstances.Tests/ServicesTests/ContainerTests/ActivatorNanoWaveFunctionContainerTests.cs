using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Services.Builders;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Collapse;

namespace Splinter.NanoInstances.Tests.ServicesTests.ContainerTests;

[TestFixture]
public class ActivatorNanoWaveFunctionContainerTests
{
    private static readonly SplinterId UnitTestAgentSplinterId1 = new()
    {
        Guid = new Guid("{EEE84800-76A3-4AAF-93C6-1F0D01D670C1}"),
        Name = "Unit Test Agent #1",
        Version = "1.0.0"
    };

    private static readonly SplinterId UnitTestAgentSplinterId2 = new()
    {
        Guid = new Guid("{A3C83875-02FC-4A38-A38F-96F14536F678}"),
        Name = "Unit Test Agent #2",
        Version = "1.0.0"
    };

    [TestCase("{EEE84800-76A3-4AAF-93C6-1F0D01D670C1}", typeof(UnitTestNanoAgent))]
    [TestCase("{A3C83875-02FC-4A38-A38F-96F14536F678}", typeof(UnitTestNanoAgent))]
    [TestCase("{81CF32F3-FECF-42DD-B9A5-4765D00D4E20}", typeof(UnitTestNanoAgentWithSplinterIds))]
    [TestCase("{413B20F0-0227-4E79-8677-DAA205918D08}", typeof(UnitTestNanoAgentWithInheritedSplinterIds))]
    public async Task Collapse_WhenProvidedWithValidParameters_Collapses(
        string guid, 
        Type expectedType)
    {
        var container = await GetDefaultContainer();
        var parameters = new NanoCollapseParameters
        {
            NanoTypeId = new Guid(guid)
        };
        var agent = await container.Collapse(parameters);

        Assert.IsInstanceOf(expectedType, agent);
    }

    [Test]
    public async Task Collapse_WhenProvidedWithInvalidParameters_ReturnsNull()
    {
        var container = await GetDefaultContainer();
        var parameters = new NanoCollapseParameters
        {
            NanoTypeId = Guid.NewGuid()
        };
        var agent = await container.Collapse(parameters);

        Assert.IsNull(agent);
    }

    [Test]
    public async Task Sync_WhenOverridingAnExistingMapping_CollapsesCorrectly()
    {
        var container = await GetDefaultContainer();
        var parameters = new NanoCollapseParameters
        {
            NanoTypeId = new Guid("{EEE84800-76A3-4AAF-93C6-1F0D01D670C1}")
        };
        var agent = await container.Collapse(parameters);

        Assert.IsInstanceOf(typeof(UnitTestNanoAgent), agent);

        parameters = parameters with {NanoTypeId = new Guid("{81CF32F3-FECF-42DD-B9A5-4765D00D4E20}")};

        await container.Synch(parameters.NanoTypeId, typeof(UnitTestNanoAgentWithSplinterIds));

        agent = await container.Collapse(parameters);

        Assert.IsInstanceOf(typeof(UnitTestNanoAgentWithSplinterIds), agent);
    }

    private static async Task<INanoWaveFunctionContainer> GetDefaultContainer()
    {
        var builder = new ActivatorNanoWaveFunctionBuilder();

        await builder.Register<UnitTestNanoAgentWithSplinterIds>();
        await builder.Register<UnitTestNanoAgentWithInheritedSplinterIds>();
        await builder.Register<UnitTestNanoAgent>(UnitTestAgentSplinterId1);
        await builder.Register<UnitTestNanoAgent>(UnitTestAgentSplinterId2);

        return await builder.Build(new TimeSpan());
    }
}