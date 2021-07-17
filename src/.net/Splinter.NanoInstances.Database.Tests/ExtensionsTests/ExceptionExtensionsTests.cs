using System;
using Splinter.NanoInstances.Database.Extensions;
using NUnit.Framework;
using Splinter.NanoInstances.Database.Tests.Utilities;

namespace Splinter.NanoInstances.Database.Tests.ExtensionsTests
{
    [TestFixture]
    public class ExceptionExtensionsTests
    {
        [TestCase(typeof(Exception))]
        [TestCase(typeof(InvalidOperationException))]
        [TestCase(typeof(ArgumentException))]
        [TestCase(typeof(ArgumentNullException))]
        public void IsDuplicateDataException_WhenProvidedWithNonDbExceptions_ReturnsFalse(Type type)
        {
            var exception = Activator.CreateInstance(type) as Exception;

            Assert.IsNotNull(exception);
            Assert.IsFalse(exception!.IsDuplicateDataException());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void IsDuplicateDataException_WhenProvidedWithASqlExceptionButIncorrectCode_ReturnsFalse(int nestedCount)
        {
            var exception = GenerateNestedSqlException(nestedCount, -1);

            Assert.IsFalse(exception.IsDuplicateDataException());
        }

        [TestCase(1, 2627)]
        [TestCase(2, 2627)]
        [TestCase(3, 2627)]
        [TestCase(4, 2627)]
        [TestCase(5, 2627)]
        [TestCase(1, 2601)]
        [TestCase(2, 2601)]
        [TestCase(3, 2601)]
        [TestCase(4, 2601)]
        [TestCase(5, 2601)]
        public void IsDuplicateDataException_WhenProvidedWithASqlExceptionButAndCorrectCode_ReturnsTrue(
            int nestedCount,
            int sqlNumber)
        {
            var exception = GenerateNestedSqlException(nestedCount, sqlNumber);

            Assert.IsTrue(exception.IsDuplicateDataException());
        }

        private static Exception GenerateNestedSqlException(int count, int sqlCode)
        {
            if (count == 1)
            {
                return MockUtilities.MockSqlException(sqlCode);
            }

            var innerException = GenerateNestedSqlException(count - 1, sqlCode);

            return new Exception("inner", innerException);
        }
    }
}
