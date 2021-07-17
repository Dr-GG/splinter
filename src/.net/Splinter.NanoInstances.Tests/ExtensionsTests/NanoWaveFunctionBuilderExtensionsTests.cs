using System;
using NUnit.Framework;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Services.Builders;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoInstances.Tests.ExtensionsTests
{
    [TestFixture]
    public class NanoWaveFunctionBuilderExtensionsTests
    {
        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(NanoWaveFunctionBuilderExtensionsTests))]
        public void Register_WhenProvidingANonNanoType_ThrowsAnException(Type type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoInstanceException>(() => builder.Register(type));
        }

        [TestCase(typeof(INanoAgent))]
        [TestCase(typeof(IUnitTestNanoAgent))]
        [TestCase(typeof(UnitTestAbstractNanoAgent))]
        public void Register_WhenProvidingAInvalidNanoType_ThrowsAnException(Type type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoInstanceException>(() => builder.Register(type));
        }

        [TestCase(typeof(UnitTestNanoAgent))]
        public void Register_WhenProvidedWithANanoInstanceWithNoStaticKey_ThrowsAnException(Type type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoTypeException>(() => builder.Register(type));
        }

        [TestCase(typeof(UnitTestNanoAgentWithOneSplinterId))]
        [TestCase(typeof(UnitTestNanoAgentWithSplinterIds))]
        [TestCase(typeof(UnitTestNanoAgentWithInheritedSplinterIds))]
        public void Register_WhenProvidedWithANanoInstanceWithStaticKey_DoesNotThrowAnException(Type type)
        {
            var builder = GetBuilder();

            Assert.DoesNotThrowAsync(() => builder.Register(type));
        }

        [TestCase("Invalid Type")]
        [TestCase("System.Object2, System")]
        [TestCase("System.String2, System")]
        public void Register_WhenProvidingAnInvalidType_ThrowsAnException(string type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoTypeException>(() => builder.Register(type));
        }

        [TestCase("System.Object, System.Private.CoreLib")]
        [TestCase("System.String, System.Private.CoreLib")]
        [TestCase("System.Int32, System.Private.CoreLib")]
        [TestCase("System.DateTime, System.Private.CoreLib")]
        [TestCase("Splinter.NanoInstances.Tests.ExtensionsTests.NanoWaveFunctionBuilderExtensionsTests, Splinter.NanoInstances.Tests")]
        public void Register_WhenProvidingAValidTypesThatAreNotNanoTypes_ThrowsAnException(string type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoInstanceException>(() => builder.Register(type));
        }

        [TestCase("Splinter.NanoTypes.Interfaces.Agents.INanoAgent, Splinter.NanoTypes")]
        [TestCase("Splinter.NanoInstances.Tests.Agents.IUnitTestNanoAgent, Splinter.NanoInstances.Tests")]
        [TestCase("Splinter.NanoInstances.Tests.Agents.UnitTestAbstractNanoAgent, Splinter.NanoInstances.Tests")]
        public void Register_WhenProvidingAValidTypeButInvalidNanoType_ThrowsAnException(string type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoInstanceException>(() => builder.Register(type));
        }

        [TestCase("Splinter.NanoInstances.Tests.Agents.UnitTestNanoAgent, Splinter.NanoInstances.Tests")]
        public void Register_WhenProvidedWithAValidTypeWithANanoInstanceWithNoStaticKey_ThrowsAnException(string type)
        {
            var builder = GetBuilder();

            Assert.ThrowsAsync<InvalidNanoTypeException>(() => builder.Register(type));
        }
        
        [TestCase("Splinter.NanoInstances.Tests.Agents.UnitTestNanoAgentWithOneSplinterId, Splinter.NanoInstances.Tests")]
        [TestCase("Splinter.NanoInstances.Tests.Agents.UnitTestNanoAgentWithSplinterIds, Splinter.NanoInstances.Tests")]
        [TestCase("Splinter.NanoInstances.Tests.Agents.UnitTestNanoAgentWithInheritedSplinterIds, Splinter.NanoInstances.Tests")]
        public void Register_WhenProvidedWithAValidTypeAndNanoInstanceWithStaticKey__DoesNotThrowAnException(string type)
        {
            var builder = GetBuilder();

            Assert.DoesNotThrowAsync(() => builder.Register(type));
        }

        private static INanoWaveFunctionBuilder GetBuilder()
        {
            return new ActivatorNanoWaveFunctionBuilder();
        }
    }
}
