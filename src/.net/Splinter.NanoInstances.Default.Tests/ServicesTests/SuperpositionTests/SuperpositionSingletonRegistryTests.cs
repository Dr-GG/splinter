using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests
{
    [TestFixture]
    public class SuperpositionSingletonRegistryTests
    {
        private const int NumberOfThreads = 10000;

        private static readonly Guid TestNanoTypeId = new("{4FD94C74-812F-4E50-B324-FB10B0942BEB}");
        private static readonly Guid TestNanoInstanceId = new("{F95DDA1D-9A35-4A1F-8C13-0234F6FD08F6}");

        [Test]
        public async Task Register_WhenRegisteringDifferentNanoInstances_RegistersTheLatestInstance()
        {
            INanoAgent? lastNanoAgent = null;
            var registry = GetRegistry();
            var singletons = GetRandomSingleton();
            var rootLock = new object();
            var tasks = singletons.Select(s =>
            {
                lock (rootLock)
                {
                    lastNanoAgent = s;

                    return registry.Register(TestNanoTypeId, s);
                }
            }).ToArray();

            Task.WaitAll(tasks);

            var nanoAgent = await registry.Fetch(TestNanoTypeId);

            Assert.IsNotNull(nanoAgent);
            Assert.IsNotNull(lastNanoAgent);
            Assert.AreEqual(lastNanoAgent!.InstanceId, nanoAgent!.InstanceId);
        }

        [Test]
        public async Task Register_WhenRegisteringSameNanoInstancesButDifferentInstances_KeepsTheOriginalSingleton()
        {
            var registry = GetRegistry();
            var singletons = GetRandomSingleton(TestNanoInstanceId).ToList();
            var originalSingleton = singletons.First();
            var tasks = singletons.Select(s => registry.Register(TestNanoTypeId, s))
                .ToArray();

            Task.WaitAll(tasks);

            var nanoAgent = await registry.Fetch(TestNanoTypeId);

            Assert.IsNotNull(nanoAgent);
            Assert.IsNotNull(originalSingleton);
            Assert.AreEqual(originalSingleton!.InstanceId.Guid, nanoAgent!.InstanceId.Guid);
        }

        private static IEnumerable<INanoAgent> GetRandomSingleton(Guid? nanoInstanceId = null)
        {
            var result = new List<INanoAgent>();

            for (var i = 0; i < NumberOfThreads; i++)
            {
                var mock = new Mock<INanoAgent>();
                var guid = nanoInstanceId ?? Guid.NewGuid();
                var nameAndVersion = mock.GetHashCode().ToString();

                    mock.Setup(n => n.InstanceId)
                    .Returns(new SplinterId
                    {
                        Name = nameAndVersion,
                        Version = nameAndVersion,
                        Guid = guid
                    });

                result.Add(mock.Object);
            }

            return result;
        }

        private static ISuperpositionSingletonRegistry GetRegistry()
        {
            return new SuperpositionSingletonRegistry(new SuperpositionSettings
            {
                RegistryTimeoutSpan = TimeSpan.FromHours(1)
            });
        }
    }
}
