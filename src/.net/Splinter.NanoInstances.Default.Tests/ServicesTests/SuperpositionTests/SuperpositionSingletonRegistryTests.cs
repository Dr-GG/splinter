using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoInstances.Default.Tests.Agents.NanoAgents;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class SuperpositionSingletonRegistryTests
{
    private const int NumberOfThreads = 10000;

    private static readonly Guid TestNanoTypeId = new("{4FD94C74-812F-4E50-B324-FB10B0942BEB}");
    private static readonly Guid TestNanoInstanceId = new("{F95DDA1D-9A35-4A1F-8C13-0234F6FD08F6}");

    [Test]
    public async Task Register_WhenRegisteringDifferentNanoInstances_RegistersTheLatestInstance()
    {
        INanoAgent lastNanoAgent = new UnitTestNanoAgent();
        var registry = GetRegistry();
        var singletons = GetRandomSingletons();
        var rootLock = new object();
        var tasks = singletons.Select(s =>
            {
                lock (rootLock)
                {
                    lastNanoAgent = s;

                    return registry.Register(TestNanoTypeId, s);
                }
            })
            .Cast<Task>()
            .ToArray();

        Task.WaitAll(tasks);

        var nanoAgent = (await registry.Fetch(TestNanoTypeId))!;

        nanoAgent.Should().NotBeNull();

        lastNanoAgent.Should().NotBeNull();
        lastNanoAgent.InstanceId.Should().Be(nanoAgent.InstanceId);
    }

    [Test]
    public async Task Register_WhenRegisteringSameNanoInstancesButDifferentInstances_KeepsTheOriginalSingleton()
    {
        var registry = GetRegistry();
        var singletons = GetRandomSingletons(TestNanoInstanceId).ToList();
        var originalSingleton = singletons.First();
        var tasks = singletons.Select(s => registry.Register(TestNanoTypeId, s))
            .Cast<Task>()
            .ToArray();

        Task.WaitAll(tasks);

        var nanoAgent = (await registry.Fetch(TestNanoTypeId))!;

        nanoAgent.Should().NotBeNull();
        originalSingleton.Should().NotBeNull();
        originalSingleton.InstanceId.Guid.Should().Be(nanoAgent.InstanceId.Guid);
    }

    [Test]
    public async Task DisposeAsync_WhenInvoked_DisposesAllSingletonReferences()
    {
        var registry = GetRegistry();
        var mockSingletons = GetRandomMockSingletons(TestNanoInstanceId).ToList();

        foreach (var singleton in mockSingletons)
        {
            await registry.Register(Guid.NewGuid(), singleton.Object);
        }

        await registry.DisposeAsync();

        foreach (var singleton in mockSingletons)
        {
            singleton.Verify(n => 
                n.Terminate(It.Is<NanoTerminationParameters>(p => p.Force)), 
                Times.Once);
        }
    }

    private static IEnumerable<Mock<INanoAgent>> GetRandomMockSingletons(Guid? nanoInstanceId = null)
    {
        var result = new List<Mock<INanoAgent>>();

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

            result.Add(mock);
        }

        return result;
    }

    private static IEnumerable<INanoAgent> GetRandomSingletons(Guid? nanoInstanceId = null)
    {
        return GetRandomMockSingletons(nanoInstanceId)
            .Select(m => m.Object)
            .ToList();
    }

    private static ISuperpositionSingletonRegistry GetRegistry()
    {
        return new SuperpositionSingletonRegistry(new SuperpositionSettings
        {
            RegistryTimeoutSpan = TimeSpan.FromHours(1)
        });
    }
}