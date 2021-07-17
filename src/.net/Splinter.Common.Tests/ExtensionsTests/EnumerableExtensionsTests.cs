using System.Collections.Generic;
using NUnit.Framework;
using Splinter.Common.Extensions;

namespace Splinter.Common.Tests.ExtensionsTests
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void IsEmpty_WhenCollectionIsNull_ReturnsTrue()
        {
            IEnumerable<object>? collection = null;

            Assert.IsTrue(collection.IsEmpty());
        }

        [Test]
        public void IsEmpty_WhenIEnumerableIsEmpty_ReturnsTrue()
        {
            var list = new List<object>();

            Assert.IsTrue(list.IsEmpty());
        }

        [Test]
        public void IsEmpty_WhenIEnumerableIsNotEmpty_ReturnsFalse()
        {
            var array = new[] {1, 2, 3};

            Assert.IsFalse(array.IsEmpty());
        }
    }
}
