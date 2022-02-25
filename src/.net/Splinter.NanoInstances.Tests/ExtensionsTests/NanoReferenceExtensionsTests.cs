using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Services.Superposition;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Tests.ExtensionsTests
{
    [TestFixture]
    public class NanoReferenceExtensionsTests
    {
        [Test]
        public void Typed_WhenNoReferenceIsSet_ThrowsAnException()
        {
            var nanoReference = new NanoReference();

            Assert.Throws<InvalidNanoInstanceException>(() => nanoReference.Typed<INanoAgent>());
        }

        [Test]
        public void Typed_WhenIncorrectReferenceIsTyped_ThrowsAnException()
        {
            var nanoReference = new NanoReference();

            nanoReference.Initialise(new UnitTestNanoAgent());

            Assert.Throws<InvalidNanoInstanceException>(() => nanoReference.Typed<ITeraAgent>());
        }

        [Test]
        public void Typed_WhenCorrectReferenceIsTyped_ThrowsNoException()
        {
            var nanoReference = new NanoReference();

            nanoReference.Initialise(new UnitTestNanoAgent());

            AssertNoException<INanoAgent>(nanoReference);
            AssertNoException<IUnitTestNanoAgent>(nanoReference);
            AssertNoException<UnitTestAbstractNanoAgent>(nanoReference);
        }

        private static void AssertNoException<TNanoType>(INanoReference reference) where TNanoType : INanoAgent
        {
            Assert.DoesNotThrow(() => reference.Typed<TNanoType>());
        }
    }
}
