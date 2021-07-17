using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Mappers;
using Splinter.NanoInstances.Tests.Mappers;
using Splinter.NanoInstances.Tests.Models;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Tests.ExtensionsTests
{
    [TestFixture]
    public class MapperExtensionsTests
    {
        [Test]
        public void IUnaryMapper_MapNullable_WhenMappingNull_ReturnsNull()
        {
            var unaryMapper = GetLeftToRightMapper();
            var result = unaryMapper.MapNullable(null);

            Assert.IsNull(result);
        }

        [Test]
        public void IUnaryMapper_MapCollection_WhenMappingCollectionIsNull_ReturnsEmpty()
        {
            var unaryMapper = GetLeftToRightMapper();
            var result = unaryMapper.MapCollection(null);

            Assert.IsEmpty(result);
        }

        [Test]
        public void IUnaryMapper_MapCollection_WhenMappingCollection_ReturnsCollection()
        {
            var unaryMapper = GetLeftToRightMapper();
            var input = new[]
            {
                new LeftModel {Property1 = 1, Property2 = "test-1"},
                new LeftModel {Property1 = 2, Property2 = "test-2"}
            };
            var result = unaryMapper.MapCollection(input).ToList();

            Assert.AreEqual(2, result.Count);

            var result1 = result[0];
            var result2 = result[1];

            Assert.AreEqual(1, result1.Property1);
            Assert.AreEqual(2, result2.Property1);

            Assert.AreEqual("test-1", result1.Property2);
            Assert.AreEqual("test-2", result2.Property2);
        }

        [Test]
        public void IBinaryMapper_MapNullable_WhenMappingNull_ReturnsNull()
        {
            var mapper = GetBinaryMapper();
            LeftModel? leftSource = null;
            RightModel? rightSource = null;

            var rightDestination = mapper.MapNullable(leftSource);
            var leftDestination = mapper.MapNullable(rightSource);

            Assert.IsNull(rightDestination);
            Assert.IsNull(leftDestination);
        }

        [Test]
        public void IBinaryMapper_MapCollection_WhenMappingCollectionIsNull_ReturnsEmpty()
        {
            var binaryMapper = GetBinaryMapper();
            IEnumerable<LeftModel>? leftCollection = null;
            IEnumerable<RightModel>? rightCollection = null;

            var rightResult = binaryMapper.MapCollection(leftCollection);
            var leftResult = binaryMapper.MapCollection(rightCollection);

            Assert.IsEmpty(leftResult);
            Assert.IsEmpty(rightResult);
        }

        [Test]
        public void IBinaryMapper_MapCollection_WhenMappingCollection_ReturnsCollection()
        {
            var binaryMapper = GetBinaryMapper();
            var leftInput = new[]
            {
                new LeftModel {Property1 = 1, Property2 = "test-1"},
                new LeftModel {Property1 = 2, Property2 = "test-2"}
            };
            var rightInput = new[]
            {
                new RightModel {Property1 = 1, Property2 = "test-1"},
                new RightModel {Property1 = 2, Property2 = "test-2"}
            };
            var leftResult = binaryMapper.MapCollection(rightInput).ToList();
            var rightResult = binaryMapper.MapCollection(leftInput).ToList();

            Assert.AreEqual(2, leftResult.Count);
            Assert.AreEqual(2, rightResult.Count);

            var leftResult1 = leftResult[0];
            var leftResult2 = leftResult[1];

            Assert.IsInstanceOf<LeftModel>(leftResult1);
            Assert.IsInstanceOf<LeftModel>(leftResult2);
            Assert.AreEqual(1, leftResult1.Property1);
            Assert.AreEqual(2, leftResult2.Property1);
            Assert.AreEqual("test-1", leftResult1.Property2);
            Assert.AreEqual("test-2", leftResult2.Property2);

            var rightResult1 = rightResult[0];
            var rightResult2 = rightResult[1];

            Assert.IsInstanceOf<RightModel>(rightResult1);
            Assert.IsInstanceOf<RightModel>(rightResult2);
            Assert.AreEqual(1, rightResult1.Property1);
            Assert.AreEqual(2, rightResult2.Property1);
            Assert.AreEqual("test-1", rightResult1.Property2);
            Assert.AreEqual("test-2", rightResult2.Property2);
        }

        private static IUnaryMapper<LeftModel, RightModel> GetLeftToRightMapper()
        {
            return new LeftToRightMapper();
        }

        private static IUnaryMapper<RightModel, LeftModel> GetRightToLeftMapper()
        {
            return new RightToLeftMapper();
        }

        private static IBinaryMapper<LeftModel, RightModel> GetBinaryMapper()
        {
            return new BinaryMapper<LeftModel, RightModel>(
                GetLeftToRightMapper(), GetRightToLeftMapper());
        }
    }
}
