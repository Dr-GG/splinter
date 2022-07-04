using System;
using NUnit.Framework;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Services.Builders;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoInstances.Tests.ExtensionsTests;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Tests.ServicesTests.BuildersTests;

[TestFixture]
public class ActivatorNanoWaveFunctionBuilderTests
{
    [TestCase(typeof(object))]
    [TestCase(typeof(string))]
    [TestCase(typeof(int))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(NanoWaveFunctionBuilderExtensionsTests))]
    public void Register_WhenProvidingANonNanoType_ThrowsAnException(Type type)
    {
        var builder = GetBuilder();

        Assert.ThrowsAsync<InvalidNanoInstanceException>(() => 
            builder.Register(new SplinterId(), type));
    }

    [TestCase(typeof(INanoAgent))]
    [TestCase(typeof(IUnitTestNanoAgent))]
    [TestCase(typeof(UnitTestAbstractNanoAgent))]
    public void Register_WhenProvidingAInvalidNanoType_ThrowsAnException(Type type)
    {
        var builder = GetBuilder();

        Assert.ThrowsAsync<InvalidNanoInstanceException>(() =>
            builder.Register(new SplinterId(), type));
    }

    [TestCase(typeof(UnitTestNanoAgent))]
    [TestCase(typeof(UnitTestNanoAgentWithOneSplinterId))]
    [TestCase(typeof(UnitTestNanoAgentWithSplinterIds))]
    [TestCase(typeof(UnitTestNanoAgentWithInheritedSplinterIds))]
    public void Register_WhenProvidedWithANanoInstanceWithStaticKey_ThrowsNoException(Type type)
    {
        var builder = GetBuilder();

        Assert.DoesNotThrowAsync(() => builder.Register(new SplinterId(), type));
    }

    private static INanoWaveFunctionBuilder GetBuilder()
    {
        return new ActivatorNanoWaveFunctionBuilder();
    }
}