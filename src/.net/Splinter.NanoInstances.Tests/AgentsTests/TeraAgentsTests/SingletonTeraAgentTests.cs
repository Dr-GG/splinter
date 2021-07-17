using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Services.Containers;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Tests.AgentsTests.TeraAgentsTests
{
    public class SingletonTeraAgentTests
    {
        private const int NumberOfThreads = 10000;

        [SetUp]
        public void SetupSplinterEnvironment()
        {
            SplinterEnvironment.TeraRegistryAgent = new Mock<ITeraRegistryAgent>().Object;
            SplinterEnvironment.TeraAgentContainer = new Mock<ITeraAgentContainer>().Object;
        }

        [TearDown]
        public void DisposeSplinterEnvironment()
        {
            SplinterEnvironment.TeraRegistryAgent = null!;
            SplinterEnvironment.TeraAgentContainer = null!;
        }

        [Test]
        public void Initialise_WhenConcurrentThreadsInitialise_InitialisesOnlyOnce()
        {
            var testAgent = new SingletonUnitTestTeraAgent();
            var range = Enumerable.Range(0, NumberOfThreads);
            var mockScope = new Mock<IServiceScope>().Object;
            var tasks = range.Select(_ => testAgent.Initialise(new NanoInitialisationParameters
                {
                    ServiceScope = mockScope,
                    Register = false,
                    RegisterNanoTable = false,
                    RegisterNanoReferences = false
                }))
                .ToArray();

            Task.WaitAll(tasks);

            Assert.AreEqual(1, testAgent.InitCount);
            Assert.AreEqual(NumberOfThreads, testAgent.AttemptInitCount);
            Assert.AreEqual(0, testAgent.AttemptDisposeCount);
            Assert.AreEqual(0, testAgent.DisposeCount);
        }

        [Test]
        public async Task Initialise_WhenConcurrentThreadsDispose_DisposeOnlyOnce()
        {
            SplinterEnvironment.Status = SplinterEnvironmentStatus.Disposing;

            var testAgent = new SingletonUnitTestTeraAgent();
            var mockScope = new Mock<IServiceScope>().Object;
            var range = Enumerable.Range(0, NumberOfThreads);
            var nanoInitParameters = new NanoInitialisationParameters
            {
                ServiceScope = mockScope,
                Register = false,
                RegisterNanoTable = false,
                RegisterNanoReferences = false
            };

            await testAgent.Initialise(nanoInitParameters);

            var tasks = range
                .Select(_ => testAgent.Dispose(new NanoDisposeParameters()))
                .ToArray();

            Task.WaitAll(tasks);

            Assert.AreEqual(1, testAgent.DisposeCount);
            Assert.AreEqual(NumberOfThreads, testAgent.AttemptDisposeCount);
            Assert.AreEqual(1, testAgent.InitCount);
            Assert.AreEqual(1, testAgent.AttemptInitCount);
        }

        [TestCase(SplinterEnvironmentStatus.Disposed)]
        [TestCase(SplinterEnvironmentStatus.Disposing)]
        [TestCase(SplinterEnvironmentStatus.Initialised)]
        [TestCase(SplinterEnvironmentStatus.Initialising)]
        [TestCase(SplinterEnvironmentStatus.Uninitialised)]
        public async Task Initialise_WhenSetToForceDispose_AlwaysDisposes(SplinterEnvironmentStatus status)
        {
            SplinterEnvironment.Status = status;

            var testAgent = new SingletonUnitTestTeraAgent();
            var mockScope = new Mock<IServiceScope>().Object;
            var range = Enumerable.Range(0, NumberOfThreads);
            var nanoInitParameters = new NanoInitialisationParameters
            {
                ServiceScope = mockScope,
                Register = false,
                RegisterNanoTable = false,
                RegisterNanoReferences = false
            };

            await testAgent.Initialise(nanoInitParameters);

            var tasks = range
                .Select(_ => testAgent.Dispose(new NanoDisposeParameters
                {
                    Force = true
                }))
                .ToArray();

            Task.WaitAll(tasks);

            Assert.AreEqual(1, testAgent.DisposeCount);
            Assert.AreEqual(NumberOfThreads, testAgent.AttemptDisposeCount);
            Assert.AreEqual(1, testAgent.InitCount);
            Assert.AreEqual(1, testAgent.AttemptInitCount);
        }

        [TestCase(SplinterEnvironmentStatus.Disposed)]
        [TestCase(SplinterEnvironmentStatus.Initialised)]
        [TestCase(SplinterEnvironmentStatus.Initialising)]
        [TestCase(SplinterEnvironmentStatus.Uninitialised)]
        public async Task Initialise_WhenConcurrentThreadsDisposeAndEnvironmentIsNotDisposing_NeverDisposes(SplinterEnvironmentStatus status)
        {
            SplinterEnvironment.Status = status;

            var testAgent = new SingletonUnitTestTeraAgent();
            var mockScope = new Mock<IServiceScope>().Object;
            var range = Enumerable.Range(0, NumberOfThreads);
            var nanoInitParameters = new NanoInitialisationParameters
            {
                ServiceScope = mockScope,
                Register = false,
                RegisterNanoTable = false,
                RegisterNanoReferences = false
            };

            await testAgent.Initialise(nanoInitParameters);

            var tasks = range
                .Select(_ => testAgent.Dispose(new NanoDisposeParameters()))
                .ToArray();

            Task.WaitAll(tasks);

            Assert.AreEqual(0, testAgent.DisposeCount);
            Assert.AreEqual(NumberOfThreads, testAgent.AttemptDisposeCount);
            Assert.AreEqual(1, testAgent.InitCount);
            Assert.AreEqual(1, testAgent.AttemptInitCount);
        }
    }
}
