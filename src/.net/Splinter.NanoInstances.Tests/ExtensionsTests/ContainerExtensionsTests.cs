using Autofac;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoInstances.Tests.Models;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Tests.ExtensionsTests
{
    [TestFixture]
    public class ContainerExtensionsTests
    {
        [Test]
        public void RegisterMappers_WhenGivenAnAssembly_RegistersAllMappers()
        {
            using var container = GetDefaultContainer();
            using var scope = container.BeginLifetimeScope();
            var leftToRightMapper = scope.Resolve<IUnaryMapper<LeftModel, RightModel>>();
            var rightToLeftMapper = scope.Resolve<IUnaryMapper<RightModel, LeftModel>>();
            var binaryMapper1 = scope.Resolve<IBinaryMapper<LeftModel, RightModel>>();
            var binaryMapper2 = scope.Resolve<IBinaryMapper<RightModel, LeftModel>>();

            Assert.IsNotNull(leftToRightMapper);
            Assert.IsNotNull(rightToLeftMapper);
            Assert.IsNotNull(binaryMapper1);
            Assert.IsNotNull(binaryMapper2);

            Assert.IsInstanceOf<IUnaryMapper<LeftModel, RightModel>>(leftToRightMapper);
            Assert.IsInstanceOf<IUnaryMapper<RightModel, LeftModel>>(rightToLeftMapper);
            Assert.IsInstanceOf<IBinaryMapper<LeftModel, RightModel>>(binaryMapper1);
            Assert.IsInstanceOf<IBinaryMapper<RightModel, LeftModel>>(binaryMapper2);
        }

        [Test]
        public void RegisterMappers_WhenGivenAnAssembly_RegistersWorkAsExpected()
        {
            using var container = GetDefaultContainer();
            using var scope = container.BeginLifetimeScope();
            var leftToRightMapper = scope.Resolve<IUnaryMapper<LeftModel, RightModel>>();
            var rightToLeftMapper = scope.Resolve<IUnaryMapper<RightModel, LeftModel>>();
            var binaryMapper1 = scope.Resolve<IBinaryMapper<LeftModel, RightModel>>();
            var binaryMapper2 = scope.Resolve<IBinaryMapper<RightModel, LeftModel>>();
            var leftInput = GetLeftInputModel();
            var rightInput = GetRightInputModel();
            var rightOutput1 = leftToRightMapper.Map(leftInput);
            var rightOutput2 = binaryMapper1.Map(leftInput);
            var rightOutput3 = binaryMapper2.Map(leftInput);
            var leftOutput1 = rightToLeftMapper.Map(rightInput);
            var leftOutput2 = binaryMapper1.Map(rightInput);
            var leftOutput3 = binaryMapper2.Map(rightInput);

            AssertOutput(rightOutput1);
            AssertOutput(rightOutput2);
            AssertOutput(rightOutput3);
            AssertOutput(leftOutput1);
            AssertOutput(leftOutput2);
            AssertOutput(leftOutput3);
        }

        private static LeftModel GetLeftInputModel()
        {
            return new()
            {
                Property1 = 1,
                Property2 = "left"
            };
        }

        private static RightModel GetRightInputModel()
        {
            return new()
            {
                Property1 = 2,
                Property2 = "right"
            };
        }

        private static void AssertOutput(LeftModel model)
        {
            Assert.AreEqual(model.Property1, 2);
            Assert.AreEqual(model.Property2, "right");
        }

        private static void AssertOutput(RightModel model)
        {
            Assert.AreEqual(model.Property1, 1);
            Assert.AreEqual(model.Property2, "left");
        }

        private static IContainer GetDefaultContainer()
        {
            var assembly = typeof(ContainerExtensionsTests).Assembly;
            var container = new ContainerBuilder();
            
            /*
             * Different assemblies will be calling RegisterMappers multiple times,
             * causing multiple registrations of binary mapper.
             *
             * Make sure that this does not cause any issues/exceptions.
             */
            for (var i = 0; i < 10; i++)
            {
                container.RegisterMappers(assembly);
            }

            return container.Build();
        }
    }
}
