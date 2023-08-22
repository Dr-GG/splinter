using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.Containers;
using Splinter.NanoInstances.Default.Tests.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Containers;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Containers;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.ContainerTests;

[TestFixture]
public class TeraAgentContainerTests
{
    private const int DefaultNumberOfTestAgents = 1000;
    private const int TestDisposeNumberOfTeraAgents = 1000;
    private const int TestDisposeThreadingNumberOfTeraAgents = 25;
    private const int TestExplicitMultiThreadCount = 5;

    [Test]
    public void Execute_WhenContainerIsNotInitialised_ThrowsException()
    {
        using var container = GetContainer();
        var errorMessage = Assert.ThrowsAsync<TeraAgentContainerNotInitialisedException>(() => container.Start())!;

        errorMessage.Should().NotBeNull();
        errorMessage.Message.Should().Be("The Tera Agent container has not yet been initialised.");
    }

    [Test]
    public async Task Execute_WhenAddingAgentsWhileContainerIsRunning_AddsAndExecutesAllAgents()
    {
        await using var container = GetContainer();
        var parameters = new TeraAgentContainerExecutionParameters();
        var agents = GetTestAgents(executionLimit: 1);

        await container.Initialise(parameters);
        await container.Start();

        foreach (var agent in agents)
        {
            await container.Register(agent);
        }

        Thread.Sleep(5000);

        container.NumberOfTeraAgents.Should().Be(DefaultNumberOfTestAgents);
        TeraAgentContainerUnitTestAgent.ExecutionHit.Should().Be(DefaultNumberOfTestAgents);
    }

    [Test]
    public async Task Execute_WhenRemovingAgentsWhileContainerIsRunning_AddsAndExecutesAllAgents()
    {
        await using var container = GetContainer();
        var parameters = new TeraAgentContainerExecutionParameters();
        var agents = GetTestAgents(executionLimit: 1).ToList();

        foreach (var agent in agents)
        {
            await container.Register(agent);
        }

        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(1000);

        foreach (var agent in agents)
        {
            await container.Dispose(agent);
        }

        Thread.Sleep(1000);

        container.NumberOfTeraAgents.Should().Be(0);
        TeraAgentContainerUnitTestAgent.ExecutionHit.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Halt_WhenStoppingTheTeraAgentContainerWhileRunning_StopsAllAgents()
    {
        await using var container = GetContainer();
        var agents = GetSlidingTestAgents().ToList();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(10)
        };

        foreach (var agent in agents)
        {
            await container.Register(agent);
        }

        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(1000);

        await container.Stop();

        TeraAgentContainerUnitTestAgent.ExecutionHit.Should().BeGreaterThan(0);
        TeraAgentContainerUnitTestAgent.CompletedHit.Should().BeGreaterThan(0);
        TeraAgentContainerUnitTestAgent.CompletedHit.Should().BeLessThan(TestDisposeNumberOfTeraAgents);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task Execute_WhenGivenIntervals_ExecutesAsExpected(int intervalMilliseconds)
    {
        await using var container = GetContainer();
        var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
        var start = DateTime.UtcNow;
        var agent = GetTestAgents(1, 5).First();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds)
        };

        await container.Register(agent);
        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(threadSleep);
        await container.Stop();

        var stop = DateTime.UtcNow;

        AssertExecutionParameters(
            agent, 
            start, 
            stop, 
            TimeSpan.FromMilliseconds(intervalMilliseconds),
            TimeSpan.FromMilliseconds(intervalMilliseconds + 100));
    }

    [TestCase(0, 10)]
    [TestCase(1, 10)]
    [TestCase(10, 10)]
    [TestCase(100, 10)]
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
        await using var container = GetContainer();
        var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
        var start = DateTime.UtcNow;
        var end = start.AddMilliseconds(5 * incrementMilliseconds + 100);
        var agent = GetTestAgents(1, 5).First();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds),
            IncrementTimestamp = TimeSpan.FromMilliseconds(incrementMilliseconds)
        };

        await container.Register(agent);
        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(threadSleep);
        await container.Stop();

        AssertExecutionParameters(
            agent,
            start,
            end,
            TimeSpan.FromMilliseconds(incrementMilliseconds),
            TimeSpan.FromMilliseconds(incrementMilliseconds));
    }

    [TestCase(0, 10)]
    [TestCase(1, 10)]
    [TestCase(10, 10)]
    [TestCase(100, 10)]
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
        await using var container = GetContainer();
        var threadSleep = intervalMilliseconds == 0 ? 100 : intervalMilliseconds * 5 + 100;
        var start = new DateTime(1983, 10, 03, 18, 00, 00);
        var end = start.AddMilliseconds(5 * incrementMilliseconds + 500);
        var agent = GetTestAgents(1, 5).First();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            StartTimestamp = start,
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(intervalMilliseconds),
            IncrementTimestamp = TimeSpan.FromMilliseconds(incrementMilliseconds)
        };

        await container.Register(agent);
        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(threadSleep);
        await container.Stop();

        AssertExecutionParameters(
            agent,
            start,
            end,
            TimeSpan.FromMilliseconds(incrementMilliseconds),
            TimeSpan.FromMilliseconds(incrementMilliseconds));
    }

    [Test]
    public async Task Execute_WhenRunningOnASingleThread_OnlyInvokesFromASingleThread()
    {
        await using var container = GetContainer();
        var agents = GetTestAgents(TestDisposeThreadingNumberOfTeraAgents, 1).ToList();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(10),
            NumberOfConcurrentThreads = 1
        };

        foreach (var agent in agents)
        {
            await container.Register(agent);
        }

        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(5000);

        await container.Stop();

        TeraAgentContainerUnitTestAgent.ExecutionHit.Should().Be(TestDisposeThreadingNumberOfTeraAgents);
        TeraAgentContainerUnitTestAgent.CompletedHit.Should().Be(TestDisposeThreadingNumberOfTeraAgents);
        TeraAgentContainerUnitTestAgent.ThreadIdCounter.Should().HaveCount(1);
    }

    [Test]
    public async Task Execute_WhenRunningOnMultipleThreads_ExecutesAsExpectedOnMultipleThreads()
    {
        await using var container = GetContainer();
        var agents = GetTestAgents(TestDisposeThreadingNumberOfTeraAgents, 1).ToList();
        var parameters = new TeraAgentContainerExecutionParameters
        {
            ExecutionIntervalTimeSpan = TimeSpan.FromMilliseconds(10),
            NumberOfConcurrentThreads = TestExplicitMultiThreadCount
        };

        foreach (var agent in agents)
        {
            await container.Register(agent);
        }

        await container.Initialise(parameters);
        await container.Start();

        Thread.Sleep(5000);

        await container.Stop();

        TeraAgentContainerUnitTestAgent.ExecutionHit.Should().Be(TestDisposeThreadingNumberOfTeraAgents);
        TeraAgentContainerUnitTestAgent.CompletedHit.Should().Be(TestDisposeThreadingNumberOfTeraAgents);
        TeraAgentContainerUnitTestAgent.ThreadIdCounter.Should().HaveCountLessOrEqualTo(TestExplicitMultiThreadCount);
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

            (current.AbsoluteTimestamp >= start && current.AbsoluteTimestamp <= end).Should().BeTrue();
            (current.RelativeTimestamp >= current.AbsoluteTimestamp).Should().BeTrue();
            (current.RelativeTimestamp >= start && current.RelativeTimestamp <= end).Should().BeTrue();

            if (i == 0)
            {
                current.RelativeTimestamp.Should().Be(current.AbsoluteTimestamp);
                current.AbsoluteTimeElapsed.Should().Be(TimeSpan.Zero);
                current.RelativeTimeElapsed.Should().Be(TimeSpan.Zero);

                continue;
            }

            var previous = executions[i - 1];

            (current.AbsoluteTimestamp == previous.AbsoluteTimestamp).Should().BeTrue();
            (current.RelativeTimeElapsed >= relativeStartTimeSpan && current.RelativeTimeElapsed <= relativeEndTimesSpan).Should().BeTrue();
        }
    }

    private static IEnumerable<ITeraAgent> GetTestAgents(
        int numberOfAgents = DefaultNumberOfTestAgents,
        int executionLimit = int.MaxValue)
    {
        TeraAgentContainerUnitTestAgent.ResetMultiThreadVariables();

        var result = new List<ITeraAgent>(numberOfAgents);

        for (var i = 0; i < numberOfAgents; i++)
        {
            result.Add(new TeraAgentContainerUnitTestAgent(executionLimit));
        }

        return result;
    }

    private static IEnumerable<ITeraAgent> GetSlidingTestAgents(
        int numberOfAgents = DefaultNumberOfTestAgents,
        int executionLimitOffset = 1)
    {
        TeraAgentContainerUnitTestAgent.ResetMultiThreadVariables();

        var result = new List<ITeraAgent>(numberOfAgents);

        for (var i = 0; i < numberOfAgents; i++)
        {
            result.Add(new TeraAgentContainerUnitTestAgent(executionLimitOffset + i));
        }

        return result;
    }

    private static ITeraAgentContainer GetContainer()
    {
        return new TeraAgentContainer();
    }
}