using System.Threading;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;

namespace Splinter.NanoInstances.Tests.ExtensionsTests
{
    [TestFixture]
    public class CancellationTokenExtensionsTests
    {
        [Test]
        public void CanContinue_WhenCancellationTokenGoesThroughLifeCycle_ReturnsCorrectValue()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            Assert.IsTrue(token.CanContinue());

            source.Cancel();

            Assert.IsFalse(token.CanContinue());
        }
    }
}
