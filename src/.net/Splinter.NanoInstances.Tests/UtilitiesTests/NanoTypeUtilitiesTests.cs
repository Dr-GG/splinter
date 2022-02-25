using System;
using NUnit.Framework;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Splinter.NanoInstances.Tests.UtilitiesTests
{
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
            Assert.IsFalse(NanoTypeUtilities.IsNanoType(type));
        }

        [TestCase(typeof(INanoAgent))]
        [TestCase(typeof(ITeraAgent))]
        [TestCase(typeof(IUnitTestNanoAgent))]
        [TestCase(typeof(UnitTestAbstractNanoAgent))]
        [TestCase(typeof(UnitTestNanoAgent))]
        public void IsNanoType_WhenProvidedWithNanoTypes_ReturnsTrue(Type type)
        {
            Assert.IsTrue(NanoTypeUtilities.IsNanoType(type));
        }

        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(NanoTypeUtilitiesTests))]
        public void IsNanoInstance_WhenProvidedWithANonNanoType_ReturnsFalse(Type type)
        {
            Assert.IsFalse(NanoTypeUtilities.IsNanoInstance(type));
        }

        [TestCase(typeof(INanoAgent))]
        [TestCase(typeof(ITeraAgent))]
        [TestCase(typeof(IUnitTestNanoAgent))]
        [TestCase(typeof(UnitTestAbstractNanoAgent))]
        public void IsNanoInstance_WhenProvidedWithNanoTypes_ReturnsFalse(Type type)
        {
            Assert.IsFalse(NanoTypeUtilities.IsNanoInstance(type));
        }

        [TestCase(typeof(UnitTestNanoAgent))]
        public void IsNanoInstance_WhenProvidedWithNanoInstances_ReturnsTrue(Type type)
        {
            Assert.IsTrue(NanoTypeUtilities.IsNanoInstance(type));
        }

        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(NanoTypeUtilitiesTests))]
        public void GetSplinterIds_WhenProvidedWithNonNanoTypes_ReturnsNull(Type type)
        {
            NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

            Assert.IsNull(nanoTypeId);
            Assert.IsNull(nanoInstanceId);
        }

        [TestCase(typeof(INanoAgent))]
        [TestCase(typeof(ITeraAgent))]
        [TestCase(typeof(IUnitTestNanoAgent))]
        [TestCase(typeof(UnitTestAbstractNanoAgent))]
        public void GetSplinterIds_WhenProvidedWithNanoTypes_ReturnsNull(Type type)
        {
            NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

            Assert.IsNull(nanoTypeId);
            Assert.IsNull(nanoInstanceId);
        }

        [TestCase(typeof(UnitTestNanoAgent))]
        public void GetSplinterIds_WhenProvidedWithInvalidNanoInstances_ReturnsNull(Type type)
        {
            NanoTypeUtilities.GetSplinterIds(type, out var nanoTypeId, out var nanoInstanceId);

            Assert.IsNull(nanoTypeId);
            Assert.IsNull(nanoInstanceId);
        }
        
        [Test]
        public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithBothIds_ReturnsTheValues()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithSplinterIds>(
                out var nanoTypeId, 
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNotNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithSplinterIds.NanoTypeId));
            Assert.IsTrue(nanoInstanceId!.Equals(UnitTestNanoAgentWithSplinterIds.NanoInstanceId));
        }

        [Test]
        public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithOneId_ReturnsValues()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithOneSplinterId>(
                out var nanoTypeId,
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithOneSplinterId.NanoTypeId));
        }

        [Test]
        public void GetSplinterIds_WhenProvidedWithAValidNanoInstanceWithInheritedIds_ReturnsTheValues()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithInheritedSplinterIds>(
                out var nanoTypeId,
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNotNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithOneSplinterId.NanoTypeId));
            Assert.IsTrue(nanoInstanceId!.Equals(UnitTestNanoAgentWithInheritedSplinterIds.NanoInstanceId));
        }

        [Test]
        public void GetSplinterIds_WhenOverridingStaticReadonlyFields_GetsTheTopFields()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewSplinterIds>(
                out var nanoTypeId,
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNotNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithNewSplinterIds.NanoTypeId));
            Assert.IsTrue(nanoInstanceId!.Equals(UnitTestNanoAgentWithNewSplinterIds.NanoInstanceId));
        }

        [Test]
        public void GetSplinterIds_WhenOverridingStaticReadonlyFieldsOnlyInBaseClass_GetsTheTopFields()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewButNotOverridingSplinterIds>(
                out var nanoTypeId,
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNotNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithNewButNotOverridingSplinterIds.NanoTypeId));
            Assert.IsTrue(nanoInstanceId!.Equals(UnitTestNanoAgentWithNewButNotOverridingSplinterIds.NanoInstanceId));
        }

        [Test]
        public void GetSplinterIds_WhenOverridingStaticReadonlyTwice_GetsTheTopFields()
        {
            NanoTypeUtilities.GetSplinterIds<UnitTestNanoAgentWithNewNewSplinterIds>(
                out var nanoTypeId,
                out var nanoInstanceId);

            Assert.IsNotNull(nanoTypeId);
            Assert.IsNotNull(nanoInstanceId);

            Assert.IsTrue(nanoTypeId!.Equals(UnitTestNanoAgentWithNewNewSplinterIds.NanoTypeId));
            Assert.IsTrue(nanoInstanceId!.Equals(UnitTestNanoAgentWithNewNewSplinterIds.NanoInstanceId));
        }
    }
}
