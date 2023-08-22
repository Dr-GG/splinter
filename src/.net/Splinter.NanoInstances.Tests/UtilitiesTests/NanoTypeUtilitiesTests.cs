using System;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Splinter.NanoInstances.Tests.UtilitiesTests;

[TestFixture]
public class NanoTypeUtilitiesTests
{
    [TestCase(typeof(object))]
    [TestCase(typeof(string))]
    [TestCase(typeof(int))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(NanoTypeUtilitiesTests))]
    public void IsNanoType_WhenProvidedWithANonNanoType_ReturnsFalse(Type type)
    {
        NanoTypeUtilities.IsNanoType(type).Should().BeFalse();
    }

    [TestCase(typeof(INanoAgent))]
    [TestCase(typeof(ITeraAgent))]
    [TestCase(typeof(IUnitTestNanoAgent))]
    [TestCase(typeof(UnitTestAbstractNanoAgent))]
    [TestCase(typeof(UnitTestNanoAgent))]
    public void IsNanoType_WhenProvidedWithNanoTypes_ReturnsTrue(Type type)
    {
        NanoTypeUtilities.IsNanoType(type).Should().BeTrue();
    }

    [TestCase(typeof(object))]
    [TestCase(typeof(string))]
    [TestCase(typeof(int))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(NanoTypeUtilitiesTests))]
    public void IsNanoInstance_WhenProvidedWithANonNanoType_ReturnsFalse(Type type)
    {
        NanoTypeUtilities.IsNanoInstance(type).Should().BeFalse();
    }

    [TestCase(typeof(INanoAgent))]
    [TestCase(typeof(ITeraAgent))]
    [TestCase(typeof(IUnitTestNanoAgent))]
    [TestCase(typeof(UnitTestAbstractNanoAgent))]
    public void IsNanoInstance_WhenProvidedWithNanoTypes_ReturnsFalse(Type type)
    {
        NanoTypeUtilities.IsNanoInstance(type).Should().BeFalse();
    }

    [TestCase(typeof(UnitTestNanoAgent))]
    public void IsNanoInstance_WhenProvidedWithNanoInstances_ReturnsTrue(Type type)
    {
        NanoTypeUtilities.IsNanoInstance(type).Should().BeTrue();
    }

    [TestCase(typeof(object))]
    [TestCase(typeof(string))]
    [TestCase(typeof(int))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(NanoTypeUtilitiesTests))]
    public void GetSplinterIds_WhenProvidedWithNonNanoTypes_ReturnsNull(Type type)
    {
        NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

        nanoTypeId.Should().BeNull();
        nanoInstanceId.Should().BeNull();
    }

    [TestCase(typeof(INanoAgent))]
    [TestCase(typeof(ITeraAgent))]
    [TestCase(typeof(IUnitTestNanoAgent))]
    [TestCase(typeof(UnitTestAbstractNanoAgent))]
    public void GetSplinterIds_WhenProvidedWithNanoTypes_ReturnsNull(Type type)
    {
        NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

        nanoTypeId.Should().BeNull();
        nanoInstanceId.Should().BeNull();
    }

    [TestCase(typeof(UnitTestNanoAgent))]
    public void GetSplinterIds_WhenProvidedWithInvalidNanoInstances_ReturnsNull(Type type)
    {
        NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

        nanoTypeId.Should().BeNull();
        nanoInstanceId.Should().BeNull();
    }
        
    [Test]
    public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithBothIds_ReturnsTheValues()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithSplinterIds>(
            out var nanoTypeId, 
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().NotBeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithSplinterIds.NanoTypeId);
        nanoInstanceId!.Should().Be(UnitTestNanoAgentWithSplinterIds.NanoInstanceId);
    }

    [Test]
    public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithOneId_ReturnsValues()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithOneSplinterId>(
            out var nanoTypeId,
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().BeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithOneSplinterId.NanoTypeId);
    }

    [Test]
    public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithInheritedIds_ReturnsTheValues()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithInheritedSplinterIds>(
            out var nanoTypeId,
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().NotBeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithOneSplinterId.NanoTypeId);
        nanoInstanceId!.Should().Be(UnitTestNanoAgentWithInheritedSplinterIds.NanoInstanceId);
    }

    [Test]
    public void GetSplinterIds_WhenOverridingStaticReadonlyFields_GetsTheTopFields()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewSplinterIds>(
            out var nanoTypeId,
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().NotBeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithNewSplinterIds.NanoTypeId);
        nanoInstanceId!.Should().Be(UnitTestNanoAgentWithNewSplinterIds.NanoInstanceId);
    }

    [Test]
    public void GetSplinterIds_WhenOverridingStaticReadonlyFieldsOnlyInBaseClass_GetsTheTopFields()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewButNotOverridingSplinterIds>(
            out var nanoTypeId,
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().NotBeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithNewButNotOverridingSplinterIds.NanoTypeId);
        nanoInstanceId!.Should().Be(UnitTestNanoAgentWithNewButNotOverridingSplinterIds.NanoInstanceId);
    }

    [Test]
    public void GetSplinterIds_WhenOverridingStaticReadonlyTwice_GetsTheTopFields()
    {
        NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewNewSplinterIds>(
            out var nanoTypeId,
            out var nanoInstanceId);

        nanoTypeId.Should().NotBeNull();
        nanoInstanceId.Should().NotBeNull();

        nanoTypeId!.Should().Be(UnitTestNanoAgentWithNewNewSplinterIds.NanoTypeId);
        nanoInstanceId!.Should().Be(UnitTestNanoAgentWithNewNewSplinterIds.NanoInstanceId);
    }
}