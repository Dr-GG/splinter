using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoInstances.Default.NanoWaveFunctions;
using Splinter.NanoInstances.Default.Tests.Agents.NanoAgents;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Services.Builders;
using Splinter.NanoInstances.Utilities;
using Splinter.NanoTypes.Default.Domain.Enums;
using Splinter.NanoTypes.Default.Domain.Exceptions.Superposition;
using Splinter.NanoTypes.Default.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Default.Tests.NanoWaveFunctionsTests;

[TestFixture]
public class SuperpositionNanoWaveFunctionTests
{
    private static readonly Guid TestTeraId = new("{9610E84E-BEC5-4265-AB0C-A9972D240A09}");
    private static readonly Guid NonExistentNanoTypeId = new ("{573BCB7E-FB5B-4AB9-99B8-FE88BC202E7F}");
    private static Guid RequestNanoTypeId = UnitTestNanoAgentAlternative1.NanoTypeId.Guid;
    private static readonly SuperpositionMapping CollapseRequestMapping = new()
    {
        Mode = SuperpositionMode.Collapse,
        Scope = SuperpositionScope.Request,
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative1).AssemblyQualifiedName!
    };
    private static readonly SuperpositionMapping CollapseSingletonMapping = new()
    {
        Mode = SuperpositionMode.Collapse,
        Scope = SuperpositionScope.Singleton,
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative2).AssemblyQualifiedName!
    };
    private static readonly SuperpositionMapping RecollapseRequestMapping = new()
    {
        Mode = SuperpositionMode.Recollapse,
        Scope = SuperpositionScope.Request,
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative3).AssemblyQualifiedName!
    };
    private static readonly SuperpositionMapping RecollapseSingletonMapping = new()
    {
        Mode = SuperpositionMode.Recollapse,
        Scope = SuperpositionScope.Singleton,
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative4).AssemblyQualifiedName!
    };

    [Test]
    public async Task Collapse_WhenNotInitialised_ReturnsNull()
    {
        var waveFunction = await GetWaveFunction();
        var parameters = GetCollapseParameters(UnitTestNanoAgentAlternative1.NanoTypeId.Guid);
        var result = await waveFunction.Collapse(parameters);

        result.Should().BeNull();
    }

    [Test]
    public async Task Collapse_WhenNanoTypeDoesNotExist_ReturnsNull()
    {
        var waveFunction = await GetWaveFunction(true);
        var parameters = GetCollapseParameters(NonExistentNanoTypeId);
        var result = await waveFunction.Collapse(parameters);

        result.Should().BeNull();
    }

    [Test]
    public async Task Collapse_WhenCollapsingARequestScopeNanoType_ReturnsTheNanoType()
    {
        var waveFunction = await GetWaveFunction(true);
        var parameters = GetCollapseParameters(RequestNanoTypeId);
        var result = await waveFunction.Collapse(parameters);
    }

    [Test]
    public async Task Recollapse_WhenNotInitialised_ReturnsNull()
    {
        var waveFunction = await GetWaveFunction();
        var parameters = GetRecollapseParameters(UnitTestNanoAgentAlternative1.NanoTypeId.Guid);
        var result = await waveFunction.Recollapse(parameters);

        result.Should().BeNull();
    }

    [Test]
    public async Task Recollapse_WhenNanoTypeDoesNotExist_ReturnsNull()
    {
        var waveFunction = await GetWaveFunction(true);
        var parameters = GetRecollapseParameters(NonExistentNanoTypeId);
        var result = await waveFunction.Recollapse(parameters);

        result.Should().BeNull();
    }

    [TestCase(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId1)]
    [TestCase(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId2)]
    [TestCase(UnitTestNanoAgentAlternativeNanoTypeIds.NanoTypeId4)]
    public async Task Recollapse_WhenRecollapseMappingIsInvalid_ThrowsException(string nanoTypeId)
    {
        var waveFunction = await GetWaveFunction(true);
        var parameters = GetRecollapseParameters(new Guid(nanoTypeId));
        var errorMessage = Assert.ThrowsAsync<InvalidNanoTypeRecollapseOperationException>(() => waveFunction.Recollapse(parameters))!;

        errorMessage.Should().NotBeNull();
    }

    private static NanoCollapseParameters GetCollapseParameters(Guid nanoTypeId)
    {
        return new NanoCollapseParameters
        {
            TeraAgentId = TestTeraId,
            NanoTypeId = nanoTypeId
        };
    }

    private static NanoRecollapseParameters GetRecollapseParameters(Guid nanoTypeId)
    {
        return new NanoSuperpositionMappingRecollapseParameters
        {
            NanoTypeId = nanoTypeId
        };
    }

    private static IServiceScope GetDefaultScope()
    {
        var result = new Mock<IServiceScope>();

        result
            .Setup(s => s.Start())
            .ReturnsAsync(GetDefaultScope);

        result
            .Setup(s => s.Resolve<ISuperpositionMappingResolver>())
            .ReturnsAsync(GetDefaultSuperpositionMappingResolver());

        result
            .Setup(s => s.Resolve<INanoWaveFunctionBuilder>())
            .ReturnsAsync(new ActivatorNanoWaveFunctionBuilder());

        return result.Object;
    }

    private static ITeraAgent GetDefaultTeraAgent()
    {
        var result = new Mock<ITeraAgent>();

        result
            .Setup(t => t.Scope)
            .Returns(GetDefaultScope);

        return result.Object;
    }

    private static ISuperpositionMappingResolver GetDefaultSuperpositionMappingResolver()
    {
        var result = new Mock<ISuperpositionMappingResolver>();

        result
            .Setup(r => r.Resolve(It.IsAny<IEnumerable<SuperpositionMapping>>()))
            .ReturnsAsync((IEnumerable<SuperpositionMapping> mappings) =>
                mappings.Select(m =>
                {
                    var nanoInstanceType = Type.GetType(m.NanoInstanceType)!;
                        
                    NanoTypeUtilities.GetSplinterIds(nanoInstanceType, out var nanoTypeId, out _);

                    return new InternalSuperpositionMapping
                    {
                        Mode = m.Mode,
                        Scope = m.Scope,
                        Description = m.Description,
                        NanoInstanceType = m.NanoInstanceType,
                        NanoTypeId = nanoTypeId!.Guid
                    };
                }));

        return result.Object;
    }

    private static ISuperpositionMappingRegistry GetMockRegistry()
    {
        var result = new Mock<ISuperpositionMappingRegistry>();

        result
            .Setup(i => i.Fetch(It.IsAny<Guid>()))
            .ReturnsAsync((Guid nanoTypeId) =>
            {
                Type? type = null;
                SuperpositionMapping? mapping = null;

                if (nanoTypeId == UnitTestNanoAgentAlternative1.NanoTypeId.Guid)
                {
                    type = typeof(UnitTestNanoAgentAlternative1);
                    mapping = CollapseRequestMapping;
                }
                else if (nanoTypeId == UnitTestNanoAgentAlternative2.NanoTypeId.Guid)
                {
                    type = typeof(UnitTestNanoAgentAlternative2);
                    mapping = CollapseSingletonMapping;
                }
                else if (nanoTypeId == UnitTestNanoAgentAlternative3.NanoTypeId.Guid)
                {
                    type = typeof(UnitTestNanoAgentAlternative3);
                    mapping = RecollapseRequestMapping;
                }
                else if (nanoTypeId == UnitTestNanoAgentAlternative4.NanoTypeId.Guid)
                {
                    type = typeof(UnitTestNanoAgentAlternative4);
                    mapping = RecollapseSingletonMapping;
                }

                if (mapping == null || type == null)
                {
                    return null;
                }

                NanoTypeUtilities.GetSplinterIds(type, out var nanoType, out _);

                if (nanoType == null)
                {
                    return null;
                }

                return new InternalSuperpositionMapping
                {
                    Mode = mapping.Mode,
                    Scope = mapping.Scope,
                    Description = mapping.Description,
                    NanoTypeId = nanoType.Guid,
                    NanoInstanceType = type.AssemblyQualifiedName!
                };
            });

        return result.Object;
    }

    private static async Task<ISuperpositionNanoWaveFunction> GetWaveFunction(bool initialise = false)
    {
        var waveFunction = new SuperpositionNanoWaveFunction(
            new SuperpositionSettings
            {
                Mappings = Enumerable.Empty<SuperpositionMapping>()
            },
            GetMockRegistry(),
            new Mock<ISuperpositionSingletonRegistry>().Object);

        if (initialise)
        {
            await waveFunction.Initialise(
                GetDefaultTeraAgent(),
                new[]
                {
                    RecollapseRequestMapping,
                    RecollapseSingletonMapping,
                    CollapseRequestMapping,
                    CollapseSingletonMapping
                });
        }

        return waveFunction;
    }
}