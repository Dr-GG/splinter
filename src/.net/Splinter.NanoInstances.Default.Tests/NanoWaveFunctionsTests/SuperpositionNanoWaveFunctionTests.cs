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
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoInstances.Default.Tests.NanoWaveFunctionsTests;

[TestFixture]
public class SuperpositionNanoWaveFunctionTests
{
    private static readonly Guid TestTeraId = new("{9610E84E-BEC5-4265-AB0C-A9972D240A09}");
    private static readonly Guid NonExistentNanoTypeId = new ("{573BCB7E-FB5B-4AB9-99B8-FE88BC202E7F}");

    private static readonly Guid CollapseRequestNanoTypeId = UnitTestNanoAgentAlternative1.NanoTypeId.Guid;
    private static readonly Guid CollapseRequestNanoInstanceId = UnitTestNanoAgentAlternative1.NanoInstanceId.Guid;

    private static readonly Guid CollapseSingletonNanoTypeId = UnitTestNanoAgentAlternative2.NanoTypeId.Guid;
    private static readonly Guid CollapseSingletonNanoInstanceId = UnitTestNanoAgentAlternative2.NanoInstanceId.Guid;

    private static readonly Guid RecollapseRequestNanoTypeId = UnitTestNanoAgentAlternative3.NanoTypeId.Guid;
    private static readonly Guid RecollapseRequestNanoInstanceId = UnitTestNanoAgentAlternative3.NanoInstanceId.Guid;

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
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative3).AssemblyQualifiedName!,
        Description = Guid.NewGuid().ToString()
    };
    private static readonly SuperpositionMapping RecollapseSingletonMapping = new()
    {
        Mode = SuperpositionMode.Recollapse,
        Scope = SuperpositionScope.Singleton,
        NanoInstanceType = typeof(UnitTestNanoAgentAlternative4).AssemblyQualifiedName!
    };

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
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var mockSuperpositionSingletonRegistry = new Mock<ISuperpositionSingletonRegistry>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, mockSuperpositionSingletonRegistry.Object);
        var parameters = GetCollapseParameters(CollapseRequestNanoTypeId);
        var result = (await waveFunction.Collapse(parameters))!;

        result.Should().NotBeNull();
        result.TypeId.Guid.Should().Be(CollapseRequestNanoTypeId);
        result.InstanceId.Guid.Should().Be(CollapseRequestNanoInstanceId);

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
        mockSuperpositionSingletonRegistry.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenCollapsingASingletonAndTheSingletonExists_ReturnsTheSingleton()
    {
        var singletonAgent = new UnitTestNanoAgentAlternative2();
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var mockSuperpositionSingletonRegistry = new Mock<ISuperpositionSingletonRegistry>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, mockSuperpositionSingletonRegistry.Object);
        var parameters = GetCollapseParameters(CollapseSingletonNanoTypeId);

        mockSuperpositionSingletonRegistry
            .Setup(x => x.Fetch(CollapseSingletonNanoTypeId))
            .ReturnsAsync(singletonAgent);

        var result = (await waveFunction.Collapse(parameters))!;

        result.Should().NotBeNull();
        result.TypeId.Guid.Should().Be(CollapseSingletonNanoTypeId);
        result.InstanceId.Guid.Should().Be(CollapseSingletonNanoInstanceId);

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
        mockSuperpositionSingletonRegistry.Verify(r => r.Fetch(CollapseSingletonNanoTypeId), Times.Once);
        mockSuperpositionSingletonRegistry.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenCollapsingASingletonAndTheSingletonDoesNotExistAndCouldNotBeCollapsed_ReturnsNull()
    {
        var mockNanoWaveFunctionBuilder = new Mock<INanoWaveFunctionBuilder>();

        mockNanoWaveFunctionBuilder
            .Setup(b => b.Build(It.IsAny<TimeSpan>()))
            .ReturnsAsync(new Mock<INanoWaveFunctionContainer>().Object);

        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var mockSuperpositionSingletonRegistry = new Mock<ISuperpositionSingletonRegistry>();
        var mockSuperpositionMappingRegistry = new Mock<ISuperpositionMappingRegistry>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object, mockNanoWaveFunctionBuilder.Object);
        var parameters = GetCollapseParameters(CollapseSingletonNanoTypeId);
        var waveFunction = await GetWaveFunction(true, teraAgent, mockSuperpositionSingletonRegistry.Object, mockSuperpositionMappingRegistry.Object);

        mockSuperpositionMappingRegistry
            .Setup(r => r.Fetch(It.IsAny<Guid>()))
            .ReturnsAsync(new InternalSuperpositionMapping
            {
                Mode = SuperpositionMode.Collapse,
                Scope = SuperpositionScope.Singleton
            });

        var result = await waveFunction.Collapse(parameters);

        result.Should().BeNull();

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
        mockSuperpositionSingletonRegistry.Verify(r => r.Fetch(CollapseSingletonNanoTypeId), Times.Once);
        mockSuperpositionSingletonRegistry.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenTheMappingExistsButHasAnInvalidScopeValue_ThrowsAnException()
    {
        var invalidScopeValue = DateTime.Today.Second;
        var mockSuperpositionMappingRegistry = new Mock<ISuperpositionMappingRegistry>();
        var parameters = GetCollapseParameters(CollapseSingletonNanoTypeId);
        var waveFunction = await GetWaveFunction(true, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);

        mockSuperpositionMappingRegistry
            .Setup(r => r.Fetch(It.IsAny<Guid>()))
            .ReturnsAsync(new InternalSuperpositionMapping
            {
                Scope = (SuperpositionScope)invalidScopeValue,
            });

        var error = Assert.ThrowsAsync<NotSupportedException>(() => waveFunction.Collapse(parameters))!;

        error.Should().NotBeNull();
        error.Message.Should().Be($"The scope {invalidScopeValue} is not supported.");
    }

    [Test]
    public async Task Collapse_WhenCollapsingASingletonAndTheSingletonDoesNotExistAndCouldBeCollapsed_ReturnsTheAgentAndRegistersTheSingleton()
    {
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var mockSuperpositionSingletonRegistry = new Mock<ISuperpositionSingletonRegistry>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, mockSuperpositionSingletonRegistry.Object);
        var parameters = GetCollapseParameters(CollapseSingletonNanoTypeId);

        mockSuperpositionSingletonRegistry
            .Setup(x => x.Register(It.IsAny<Guid>(), It.IsAny<INanoAgent>()))
            .ReturnsAsync((Guid _, INanoAgent nanoAgent) => nanoAgent);

        var result = (await waveFunction.Collapse(parameters))!;

        result.Should().NotBeNull();
        result.TypeId.Guid.Should().Be(CollapseSingletonNanoTypeId);
        result.InstanceId.Guid.Should().Be(CollapseSingletonNanoInstanceId);

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
        mockSuperpositionSingletonRegistry.Verify(r => r.Fetch(CollapseSingletonNanoTypeId), Times.Once);
        mockSuperpositionSingletonRegistry.Verify(r => r.Register(CollapseSingletonNanoTypeId, It.IsAny<INanoAgent>()), Times.Once);
        mockSuperpositionSingletonRegistry.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenTheMappingIsRecollapsableAndThereIsNoAgentCollapsed_DoesNotRegisterTheDependency()
    {
        var mockNanoWaveFunctionBuilder = new Mock<INanoWaveFunctionBuilder>();

        mockNanoWaveFunctionBuilder
            .Setup(b => b.Build(It.IsAny<TimeSpan>()))
            .ReturnsAsync(new Mock<INanoWaveFunctionContainer>().Object);

        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object, mockNanoWaveFunctionBuilder.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent);
        var parameters = GetCollapseParameters(RecollapseRequestNanoTypeId);

        var result = await waveFunction.Collapse(parameters);

        result.Should().BeNull();

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenTheMappingIsRecollapsableAndThereIsNoTeraIdAttached_DoesNotRegisterTheDependency()
    {
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent);
        var parameters = GetCollapseParameters(RecollapseRequestNanoTypeId);

        parameters = parameters with
        {
            TeraAgentId = null
        };

        var result = (await waveFunction.Collapse(parameters))!;

        result.Should().NotBeNull();
        result.TypeId.Guid.Should().Be(RecollapseRequestNanoTypeId);
        result.InstanceId.Guid.Should().Be(RecollapseRequestNanoInstanceId);

        mockNanoTypeDependencyService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Collapse_WhenTheMappingIsRecollapsableAndThereIsNoTeraIdAttached_RegistersTheDependency()
    {
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockNanoTypeDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent);
        var parameters = GetCollapseParameters(RecollapseRequestNanoTypeId);

        var result = (await waveFunction.Collapse(parameters))!;

        result.Should().NotBeNull();
        result.TypeId.Guid.Should().Be(RecollapseRequestNanoTypeId);
        result.InstanceId.Guid.Should().Be(RecollapseRequestNanoInstanceId);

        mockNanoTypeDependencyService
            .Verify(s =>
                s.IncrementTeraAgentNanoTypeDependencies(
                    parameters.TeraAgentId!.Value, 
                    It.Is<SplinterId>(id => id.Guid == RecollapseRequestNanoTypeId),
                    1), Times.Once);
        mockNanoTypeDependencyService.VerifyNoOtherCalls();
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

    [Test]
    public async Task Recollapse_WhenMappingIsNotFound_ReturnsNull()
    {
        var mockSuperpositionMappingRegistry = new Mock<ISuperpositionMappingRegistry>();

        var waveFunction = await GetWaveFunction(true, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(NonExistentNanoTypeId);
        var result = await waveFunction.Recollapse(parameters);

        result.Should().BeNull();
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFoundAndValid_ReturnsTheCorrectRecollapseOperationId()
    {
        var recollapseId = Guid.NewGuid();
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);

        mockDependencyService
            .Setup(s => s.SignalNanoTypeRecollapses(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>?>()))
            .ReturnsAsync(recollapseId);

        var result = (await waveFunction.Recollapse(parameters))!;

        result.Should().NotBeNull();
        result.Should().Be(recollapseId);
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFoundAndValidAndNoSourceTeraIdIsProvided_UsesTheTeraParentId()
    {
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);
        
        await waveFunction.Recollapse(parameters);

        mockDependencyService            
            .Verify(s => 
                s.SignalNanoTypeRecollapses(TestTeraId, RecollapseRequestNanoTypeId, parameters.TeraIds), Times.Once);
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFoundAndValidAndASourceTeraIdIsProvided_UsesTheProvidedSourceTeraId()
    {
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);

        parameters = parameters with
        {
            SourceTeraId = Guid.NewGuid()
        };

        await waveFunction.Recollapse(parameters);

        mockDependencyService
            .Verify(s =>
                s.SignalNanoTypeRecollapses(parameters.SourceTeraId.Value, RecollapseRequestNanoTypeId, parameters.TeraIds), Times.Once);
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFound_MapsAndSyncsTheCorrectNewMapping()
    {
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);

        await waveFunction.Recollapse(parameters);

        mockSuperpositionMappingRegistry
            .Verify(r => r.Synch(It.Is<InternalSuperpositionMapping>(m =>
                m.NanoTypeId == RecollapseRequestNanoTypeId
                && m.Mode == SuperpositionMode.Recollapse
                && m.Scope == parameters.SuperpositionMapping.Scope
                && m.NanoInstanceType == parameters.SuperpositionMapping.NanoInstanceType
                && m.Description == parameters.SuperpositionMapping.Description)), Times.Once);
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFoundButNewMappingHasNoDescription_UsesTheOldMappingDescription()
    {
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);

        parameters = parameters with
        {
            SuperpositionMapping = parameters.SuperpositionMapping with
            {
                Description = null
            }
        };

        await waveFunction.Recollapse(parameters);

        mockSuperpositionMappingRegistry
            .Verify(r => r.Synch(It.Is<InternalSuperpositionMapping>(m =>
                m.Description == RecollapseRequestMapping.Description)), Times.Once);
    }

    [Test]
    public async Task Recollapse_WhenMappingIsFoundButInvalidNewNanoInstanceTypeIsProvided_ThrowsAnError()
    {
        var mockSuperpositionMappingRegistry = GetDefaultMockRegistry();
        var mockDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var teraAgent = GetDefaultTeraAgent(mockDependencyService.Object);
        var waveFunction = await GetWaveFunction(true, teraAgent, superpositionMappingRegistry: mockSuperpositionMappingRegistry.Object);
        var parameters = GetRecollapseParameters(RecollapseRequestNanoTypeId);

        parameters = parameters with
        {
            SuperpositionMapping = parameters.SuperpositionMapping with
            {
                NanoInstanceType = "InvalidNanoInstanceType"
            }
        };

        var error = Assert.ThrowsAsync<InvalidNanoInstanceException>(() => waveFunction.Recollapse(parameters))!;

        error.Should().NotBeNull();
        error.Message.Should().Be("Could not find the nano instance type InvalidNanoInstanceType.");
    }

    [Test]
    public async Task DisposeAsync_WhenInvoked_InvokesTheRegistryDispose()
    {
        var mockSingletonRegistry = new Mock<ISuperpositionSingletonRegistry>();
        var waveFunction = await GetWaveFunction(true, superpositionSingletonRegistry: mockSingletonRegistry.Object);

        await waveFunction.DisposeAsync();

        mockSingletonRegistry.Verify(r => r.DisposeAsync(), Times.Once);
    }

    private static NanoCollapseParameters GetCollapseParameters(Guid nanoTypeId)
    {
        return new NanoCollapseParameters
        {
            TeraAgentId = TestTeraId,
            NanoTypeId = nanoTypeId
        };
    }

    private static NanoSuperpositionMappingRecollapseParameters GetRecollapseParameters(Guid nanoTypeId)
    {
        return new NanoSuperpositionMappingRecollapseParameters
        {
            NanoTypeId = nanoTypeId,
            SuperpositionMapping = new SuperpositionMapping
            {
                Mode = SuperpositionMode.Collapse,
                NanoInstanceType = typeof(UnitTestNanoAgentAlternative1).AssemblyQualifiedName ?? string.Empty,
                Description = Guid.NewGuid().ToString()
            }
        };
    }

    private static IServiceScope GetDefaultScope(
        ITeraAgentNanoTypeDependencyService? teraAgentNanoTypeDependencyService = null,
        INanoWaveFunctionBuilder? nanoWaveFunctionBuilder = null)
    {
        var result = new Mock<IServiceScope>();

        teraAgentNanoTypeDependencyService ??= new Mock<ITeraAgentNanoTypeDependencyService>().Object;
        nanoWaveFunctionBuilder ??= new ActivatorNanoWaveFunctionBuilder();

        result
            .Setup(s => s.Start())
            .ReturnsAsync(result.Object);

        result
            .Setup(s => s.Resolve<ISuperpositionMappingResolver>())
            .ReturnsAsync(GetDefaultSuperpositionMappingResolver());

        result
            .Setup(s => s.Resolve<INanoWaveFunctionBuilder>())
            .ReturnsAsync(nanoWaveFunctionBuilder);

        result
            .Setup(s => s.Resolve<ITeraAgentNanoTypeDependencyService>())
            .ReturnsAsync(teraAgentNanoTypeDependencyService);

        return result.Object;
    }

    private static ITeraAgent GetDefaultTeraAgent(
        ITeraAgentNanoTypeDependencyService? teraAgentNanoTypeDependencyService = null,
        INanoWaveFunctionBuilder? nanoWaveFunctionBuilder = null)
    {
        var result = new Mock<ITeraAgent>();

        result
            .Setup(t => t.Scope)
            .Returns(GetDefaultScope(
                teraAgentNanoTypeDependencyService,
                nanoWaveFunctionBuilder));

        result
            .SetupGet(t => t.TeraId)
            .Returns(TestTeraId);

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

    private static Mock<ISuperpositionMappingRegistry> GetDefaultMockRegistry()
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

        return result;
    }

    private static ISuperpositionMappingRegistry GetDefaultRegistry()
    {
        return GetDefaultMockRegistry().Object;
    }

    private static async Task<ISuperpositionNanoWaveFunction> GetWaveFunction(
        bool initialise = false,
        ITeraAgent? teraAgent  = null,
        ISuperpositionSingletonRegistry? superpositionSingletonRegistry = null,
        ISuperpositionMappingRegistry? superpositionMappingRegistry = null)
    {
        superpositionSingletonRegistry ??= new Mock<ISuperpositionSingletonRegistry>().Object;
        superpositionMappingRegistry ??= GetDefaultRegistry();

        var waveFunction = new SuperpositionNanoWaveFunction(
            new SuperpositionSettings
            {
                Mappings = Enumerable.Empty<SuperpositionMapping>()
            },
            superpositionMappingRegistry,
            superpositionSingletonRegistry);

        if (!initialise)
        {
            return waveFunction;
        }

        teraAgent ??= GetDefaultTeraAgent();

        await waveFunction.Initialise(
            teraAgent,
            new[]
            {
                RecollapseRequestMapping,
                RecollapseSingletonMapping,
                CollapseRequestMapping,
                CollapseSingletonMapping
            });

        return waveFunction;
    }
}