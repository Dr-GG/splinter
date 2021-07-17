﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.Containers;
using Splinter.NanoInstances.Default.Tests.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Containers;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Containers;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.ContainerTests
{
    [TestFixture]
    public class TeraAgentContainerTests
    {
        private const int DefaultNumberOfTestAgents = 10000;
        private const int TestDisposeNumberOfTeraAgents = 25000;

        [Test]
        public void Execute_WhenContainerIsNotInitialised_ThrowsException()
        {
            var container = GetContainer();

            Assert.ThrowsAsync<TeraAgentContainerNotInitialisedException>(() => container.Execute());
        }

        [Test]
        public async Task Execute_WhenAddingAgentsWhileContainerIsRunning_AddsAndExecutesAllAgents()
        {
            var container = GetContainer();
            var parameters = new TeraAgentContainerExecutionParameters();
            var agents = GetTestAgents(executionLimit: 1);

            await container.Initialise(parameters);
            await container.Execute();

            foreach (var agent in agents)
            {
                await container.Register(agent);
            }

            Thread.Sleep(1000);

            Assert.AreEqual(DefaultNumberOfTestAgents, container.NumberOfTeraAgents);
            Assert.AreEqual(DefaultNumberOfTestAgents, TeraAgentContainerUnitTestAgent.ExecutionHit);
        }

        [Test]
        public async Task Execute_WhenRemovingAgentsWhileContainerIsRunning_AddsAndExecutesAllAgents()
        {
            var container = GetContainer();
            var parameters = new TeraAgentContainerExecutionParameters();
            var agents = GetTestAgents(executionLimit: 1).ToList();

            foreach (var agent in agents)
            {
                await container.Register(agent);
            }

            await container.Initialise(parameters);
            await container.Execute();

            Thread.Sleep(1000);

            foreach (var agent in agents)
            {
                await container.Dispose(agent);
            }

            Thread.Sleep(1000);
            Assert.AreEqual(0, container.NumberOfTeraAgents);
            Assert.IsTrue(TeraAgentContainerUnitTestAgent.ExecutionHit > 0);
        }

        [Test]
        public async Task Halt_WhenStoppingTheTeraAgentContainerWhileRunning_StopsAllAgents()
        {
            var container = GetContainer();
            var parameters = new TeraAgentContainerExecutionParameters();
            var agents = GetTestAgents(TestDisposeNumberOfTeraAgents, 1).ToList();

            foreach (var agent in agents)
            {
                await container.Register(agent);
            }

            await container.Initialise(parameters);
            await container.Execute();

            Thread.Sleep(1000);

            await container.Halt();

            Thread.Sleep(5000);

            Assert.IsTrue(TeraAgentContainerUnitTestAgent.ExecutionHit < TestDisposeNumberOfTeraAgents);
            Assert.AreEqual(TestDisposeNumberOfTeraAgents, container.NumberOfTeraAgents);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public async Task Execute_WhenGivenIntervals_ExecutesAsExpected(int intervalMilliseconds)
        {
            var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
            var start = DateTime.UtcNow;
            var container = GetContainer();
            var agent = GetTestAgents(1, 5).First();
            var parameters = new TeraAgentContainerExecutionParameters
            {
                ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds)
            };

            await container.Register(agent);
            await container.Initialise(parameters);
            await container.Execute();

            Thread.Sleep(threadSleep);
            await container.Halt();

            var stop = DateTime.UtcNow;

            AssertExecutionParameters(
                agent, 
                start, 
                stop, 
                TimeSpan.FromMilliseconds(intervalMilliseconds),
                TimeSpan.FromMilliseconds(intervalMilliseconds + 100));
        }

        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(10, 1)]
        [TestCase(100, 1)]
        [TestCase(1000, 1)]
        [TestCase(0, 100)]
        [TestCase(1, 100)]
        [TestCase(10, 100)]
        [TestCase(100, 100)]
        [TestCase(1000, 100)]
        [TestCase(0, 86400000)]
        [TestCase(1, 86400000)]
        [TestCase(10, 86400000)]
        [TestCase(100, 86400000)]
        [TestCase(1000, 86400000)]
        public async Task Execute_WhenGivenIncrement_ExecutesAsExpected(
            int intervalMilliseconds,
            int incrementMilliseconds)
        {
            var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
            var start = DateTime.UtcNow;
            var end = start.AddMilliseconds(5 * incrementMilliseconds + 100);
            var container = GetContainer();
            var agent = GetTestAgents(1, 5).First();
            var parameters = new TeraAgentContainerExecutionParameters
            {
                ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds),
                IncrementTimestamp = TimeSpan.FromMilliseconds(incrementMilliseconds)
            };

            await container.Register(agent);
            await container.Initialise(parameters);
            await container.Execute();

            Thread.Sleep(threadSleep);
            await container.Halt();

            AssertExecutionParameters(
                agent,
                start,
                end,
                TimeSpan.FromMilliseconds(incrementMilliseconds),
                TimeSpan.FromMilliseconds(incrementMilliseconds));
        }

        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(10, 1)]
        [TestCase(100, 1)]
        [TestCase(1000, 1)]
        [TestCase(0, 100)]
        [TestCase(1, 100)]
        [TestCase(10, 100)]
        [TestCase(100, 100)]
        [TestCase(1000, 100)]
        [TestCase(0, 86400000)]
        [TestCase(1, 86400000)]
        [TestCase(10, 86400000)]
        [TestCase(100, 86400000)]
        [TestCase(1000, 86400000)]
        public async Task Execute_WhenGivenStartDateAndIncrement_ExecutesAsExpected(
            int intervalMilliseconds,
            int incrementMilliseconds)
        {
            var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
            var start = new DateTime(1983, 10, 03, 18, 00, 00);
            var end = start.AddMilliseconds(5 * incrementMilliseconds);
            var container = GetContainer();
            var agent = GetTestAgents(1, 5).First();
            var parameters = new TeraAgentContainerExecutionParameters
            {
                StartTimestamp = start,
                ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds),
                IncrementTimestamp = TimeSpan.FromMilliseconds(incrementMilliseconds)
            };

            await container.Register(agent);
            await container.Initialise(parameters);
            await container.Execute();

            Thread.Sleep(threadSleep);
            await container.Halt();

            AssertExecutionParameters(
                agent,
                start,
                end,
                TimeSpan.FromMilliseconds(incrementMilliseconds),
                TimeSpan.FromMilliseconds(incrementMilliseconds));
        }

        private static void AssertExecutionParameters(
            ITeraAgent agent,
            DateTime start,
            DateTime end,
            TimeSpan relativeStartTimeSpan,
            TimeSpan relativeEndTimesSpan)
        {
            var executions = ((TeraAgentContainerUnitTestAgent) agent).ExecutionParameters.ToList();

            for (var i = 0; i < executions.Count; i++)
            {
                var current = executions[i];

                Assert.IsTrue(current.AbsoluteTimestamp >= start 
                              && current.AbsoluteTimestamp <= end);
                Assert.IsTrue(current.RelativeTimestamp >= current.AbsoluteTimestamp);
                Assert.IsTrue(current.RelativeTimestamp >= start
                              && current.RelativeTimestamp <= end);

                if (i == 0)
                {
                    Assert.AreEqual(current.AbsoluteTimestamp, current.RelativeTimestamp);
                    Assert.AreEqual(TimeSpan.Zero, current.AbsoluteTimeElapsed);
                    Assert.AreEqual(TimeSpan.Zero, current.RelativeTimeElapsed);

                    continue;
                }

                var previous = executions[i - 1];

                Assert.IsTrue(current.AbsoluteTimestamp == previous.AbsoluteTimestamp);
                Assert.IsTrue(current.RelativeTimeElapsed >= relativeStartTimeSpan
                              && current.RelativeTimeElapsed <= relativeEndTimesSpan);
            }
        }

        private static IEnumerable<ITeraAgent> GetTestAgents(
            int numberOfAgents = DefaultNumberOfTestAgents,
            int executionLimit = int.MaxValue)
        {
            TeraAgentContainerUnitTestAgent.ExecutionHit = 0;

            var result = new List<ITeraAgent>(numberOfAgents);

            for (var i = 0; i < numberOfAgents; i++)
            {
                result.Add(new TeraAgentContainerUnitTestAgent(executionLimit));
            }

            return result;
        }

        private static ITeraAgentContainer GetContainer()
        {
            return new TeraAgentContainer();
        }
    }
}
