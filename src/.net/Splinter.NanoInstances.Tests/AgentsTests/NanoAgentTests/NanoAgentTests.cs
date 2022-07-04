using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Tests.AgentsTests.NanoAgentTests;

[TestFixture]
public class NanoAgentTests
{
    [Test]
    public void HasTeraParent_WhenNoTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        Assert.IsFalse(result.HasTeraParent);
    }

    [Test]
    public void HasNoTeraParent_WhenNoTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent();

        Assert.IsTrue(result.HasNoTeraParent);
    }

    [Test]
    public void HasTeraParent_WhenTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        Assert.IsTrue(result.HasTeraParent);
    }

    [Test]
    public void HasNoTeraParent_WhenTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        Assert.IsFalse(result.HasNoTeraParent);
    }

    [Test]
    public void TeraParent_WhenNoTeraParentIsSet_ThrowsException()
    {
        var result = GetNanoAgent();

        Assert.Throws<TeraParentNotInitialisedException>(() => _ = result.TeraParent);
    }

    [Test]
    public void TeraParent_WhenTeraParentIsSet_ThrowsNoException()
    {
        var result = GetNanoAgent(true);

        Assert.DoesNotThrow(() => _ = result.TeraParent);
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsNotSet_ReturnsTrue()
    {
        var result = GetNanoAgent();

        Assert.IsTrue(result.HasNoNanoTable);
    }

    [Test]
    public void HasNanoTable_WhenParentIsNotSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        Assert.IsFalse(result.HasNanoTable);
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        Assert.IsTrue(result.HasNoNanoTable);
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        Assert.IsFalse(result.HasNanoTable);
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);
             
        Assert.IsFalse(result.HasNoNanoTable);
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);

        Assert.IsTrue(result.HasNanoTable);
    }
        
    [Test]
    public void NanoTable_WhenNoParentIsSet_ThrowsException()
    {
        var result = GetNanoAgent();

        Assert.Throws<NanoTableNotInitialisedException>(() => _ = result.NanoTable);
    }

    [Test]
    public void NanoTable_WhenParentIsSetWithNoNanoTable_ThrowsException()
    {
        var result = GetNanoAgent(true);

        Assert.Throws<NanoTableNotInitialisedException>(() => _ = result.NanoTable);
    }

    [Test]
    public void NanoTable_WhenParentIsSetWithNanoTable_ThrowsNoException()
    {
        var result = GetNanoAgent(true, true);

        Assert.DoesNotThrow(() => _ = result.NanoTable);
    }

    private static INanoAgent GetNanoAgent(
        bool setParent = false,
        bool parentHasNanoTable = false)
    {
        var result = new UnitTestNanoAgentWithSplinterIds();

        if (!setParent)
        {
            return result;
        }

        result.TeraParent = GetTeraParent(
            parentHasNanoTable);

        return result;
    }

    private static ITeraAgent GetTeraParent(bool hasNanoTable)
    {
        var result = new Mock<ITeraAgent>();

        result.Setup(t => t.HasNanoTable).Returns(hasNanoTable);
        result.Setup(t => t.HasNoNanoTable).Returns(!hasNanoTable);

        return result.Object;
    }
}