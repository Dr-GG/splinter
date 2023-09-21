using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoTypes.Default.Domain.Enums;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class SuperpositionMappingRegistryTests
{
    private const int NumberOfRegisterThreads = 10000;
    private static readonly Guid TestNanoTypeId = new("{B0E820EA-C9EE-4BF4-951D-CA1AB35A9701}");

    [Test]
    public async Task Register_WhenMappingsIsEmpty_SavesNothing()
    {
        var registry = GetRegistry();

        await registry.Register(Array.Empty<InternalSuperpositionMapping>());

        var mapping = await registry.Fetch(TestNanoTypeId);

        mapping.Should().BeNull();
    }

    [Test]
    public async Task Register_WhenRegisteringTheSameMappingConcurrently_SavesCorrectly()
    {
        InternalSuperpositionMapping lastMapping = new();
        var randomMappings = GetRandomMappings();
        var registry = GetRegistry();
        var rootLock = new object();
        var tasks = randomMappings.Select(m => 
        {
            lock (rootLock)
            {
                lastMapping = m;

                return registry.Register(m);
            }
        }).ToArray();

        Task.WaitAll(tasks);

        var mapping = (await registry.Fetch(TestNanoTypeId))!;

        mapping.Should().NotBeNull();

        lastMapping.Should().NotBeNull();
        lastMapping.NanoTypeId.Should().Be(mapping.NanoTypeId);
        lastMapping.NanoInstanceType.Should().Be(mapping.NanoInstanceType);
        lastMapping.Description.Should().Be(mapping.Description);
        lastMapping.Mode.Should().Be(mapping.Mode);
    }

    [Test]
    public async Task Sync_WhenSyncingAnExistingMappingConcurrently_SyncsCorrectly()
    {
        InternalSuperpositionMapping lastMapping = new();
        var randomMappings = GetRandomMappings().ToList();
        var firstMapping = randomMappings.First();
        var restOfMappings = randomMappings.Skip(1).ToList();
        var registry = GetRegistry();
        var rootLock = new object();

        await registry.Register(firstMapping);

        var tasks = restOfMappings.Select(m =>
        {
            lock (rootLock)
            {
                lastMapping = m;

                return registry.Synch(m);
            }
        }).ToArray();

        Task.WaitAll(tasks);
        
        var mapping = (await registry.Fetch(TestNanoTypeId))!;

        mapping.Should().NotBeNull();

        lastMapping.Should().NotBeNull();
        lastMapping.NanoTypeId.Should().Be(mapping.NanoTypeId);
        lastMapping.NanoInstanceType.Should().Be(mapping.NanoInstanceType);
        lastMapping.Description.Should().Be(mapping.Description);
        lastMapping.Mode.Should().Be(mapping.Mode);
    }

    private static IEnumerable<InternalSuperpositionMapping> GetRandomMappings()
    {
        var result = new List<InternalSuperpositionMapping>();

        for (var i = 0; i < NumberOfRegisterThreads; i++)
        {
            var nanoInstanceId = Guid.NewGuid();
            var mapping = new InternalSuperpositionMapping
            {
                Description = nanoInstanceId.ToString(),
                NanoInstanceType = nanoInstanceId.ToString(),
                Mode = SuperpositionMode.Collapse,
                Scope = SuperpositionScope.Request,
                NanoTypeId = TestNanoTypeId
            };

            result.Add(mapping);
        }

        return result;
    }

    private static ISuperpositionMappingRegistry GetRegistry()
    {
        return new SuperpositionMappingRegistry(new SuperpositionSettings
        {
            RegistryTimeoutSpan = TimeSpan.FromHours(1)
        });
    }
}