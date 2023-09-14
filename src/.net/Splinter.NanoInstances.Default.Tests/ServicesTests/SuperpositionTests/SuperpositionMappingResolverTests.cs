using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoInstances.Default.Tests.Agents.Knowledge;
using Splinter.NanoTypes.Default.Domain.Enums;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.SuperpositionTests;

public class SuperpositionMappingResolverTests
{
    [Test]
    public void Resolve_WhenProvidingAnInvalidType_ThrowsAnException()
    {
        var resolver = GetResolver();
        var error = Assert.ThrowsAsync<InvalidNanoTypeException>(() => resolver.Resolve(new[]
        {
            new SuperpositionMapping
            {
                NanoInstanceType = "incorrect-type"
            }
        }))!;

        error.Should().NotBeNull();
        error.Message.Should().Be("Could not find the nano type incorrect-type.");
    }

    [Test]
    public void Resolve_WhenProvidingATypeThatDoesNotHaveSplinterIds_ThrowsAnException()
    {
        var resolver = GetResolver();
        var error = Assert.ThrowsAsync<InvalidNanoTypeException>(() => resolver.Resolve(new[]
        {
            new SuperpositionMapping
            {
                NanoInstanceType = typeof(string).AssemblyQualifiedName ?? ""
            }
        }))!;

        error.Should().NotBeNull();
        error.Message.Should().Be("The nano type System.String does not consist of a static 'NanoTypeId' field.");
    }

    [Test]
    public async Task Resolve_WhenProvidingAValidType_MapsTheMappings()
    {
        var resolver = GetResolver();
        var inputMappings = new[]
        {
            new SuperpositionMapping
            {
                NanoInstanceType = typeof(AfrikaansLanguageAgent).AssemblyQualifiedName ?? "",
                Description = Guid.NewGuid().ToString(),
                Mode = SuperpositionMode.Collapse,
                Scope = SuperpositionScope.Request
            },

            new SuperpositionMapping
            {
                NanoInstanceType = typeof(EnglishLanguageAgent).AssemblyQualifiedName ?? "",
                Mode = SuperpositionMode.Recollapse,
                Scope = SuperpositionScope.Singleton
            },
        };
        var result = await resolver.Resolve(inputMappings);
        var expected = new[]
        {
            new InternalSuperpositionMapping
            {
                Description = inputMappings[0].Description,
                Mode = inputMappings[0].Mode,
                Scope = inputMappings[0].Scope,
                NanoInstanceType = inputMappings[0].NanoInstanceType,
                NanoTypeId = LanguageAgents.NanoTypeId.Guid
            },

            new InternalSuperpositionMapping
            {
                Description = inputMappings[1].Description,
                Mode = inputMappings[1].Mode,
                Scope = inputMappings[1].Scope,
                NanoInstanceType = inputMappings[1].NanoInstanceType,
                NanoTypeId = LanguageAgents.NanoTypeId.Guid
            }
        };

        result.Should().BeEquivalentTo(expected);
    }

    private static ISuperpositionMappingResolver GetResolver()
    {
        return new SuperpositionMappingResolver();
    }
}
