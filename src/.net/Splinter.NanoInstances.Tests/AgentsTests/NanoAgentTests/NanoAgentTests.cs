using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Tests.AgentsTests.NanoAgentTests;

[TestFixture]
public class NanoAgentTests
{
    [Test]
    public void HasTeraParent_WhenNoTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        result.HasTeraParent.Should().BeFalse();
    }

    [Test]
    public void HasNoTeraParent_WhenNoTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent();

        result.HasNoTeraParent.Should().BeTrue();
    }

    [Test]
    public void HasTeraParent_WhenTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        result.HasTeraParent.Should().BeTrue();
    }

    [Test]
    public void HasNoTeraParent_WhenTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        result.HasNoTeraParent.Should().BeFalse();
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

        result.HasNoNanoTable.Should().BeTrue();
    }

    [Test]
    public void HasNanoTable_WhenParentIsNotSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        result.HasNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        result.HasNoNanoTable.Should().BeTrue();
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        result.HasNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);

        result.HasNoNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);

        result.HasNanoTable.Should().BeTrue();
    }
        
    [Test]
    public void NanoTable_WhenNoParentIsSet_ThrowsException()
    {
        var result = GetNanoAgent();

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => _ = result.NanoTable);
    }

    [Test]
    public void NanoTable_WhenParentIsSetWithNoNanoTable_ThrowsException()
    {
        var result = GetNanoAgent(true);

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => _ = result.NanoTable);
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

        result.TeraParent = GetTeraParent(parentHasNanoTable);

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