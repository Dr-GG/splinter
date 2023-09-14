using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using System;
using System.Threading.Tasks;
using Moq;
using Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Default.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Splinter.NanoTypes.Interfaces.WaveFunctions;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Tests.AgentsTests.TeraAgentsTests.SuperpositionTests;

public class SuperpositionAgentTests
{
    [Test]
    public void TypeId_WhenCalled_ReturnsTheDefaultNanoTypeId()
    {
        var agent = GetSuperpositionAgent();

        agent.TypeId.Should().Be(SplinterIdConstants.SuperpositionAgentNanoTypeId);
    }

    [Test]
    public void InstanceId_WhenCalled_ReturnsTheExpectedNanoInstanceId()
    {
        var agent = GetSuperpositionAgent();

        agent.InstanceId.Guid.Should().Be(new Guid("{F253ADF7-6295-47B1-9DE2-5C7980AB125C}"));
    }

    [Test]
    public async Task Initialise_WhenInvoked_InitialisesTheAgent()
    {
        var agent = GetSuperpositionAgent();
        var mockWaveFunction = new Mock<ISuperpositionNanoWaveFunction>();
        var mockScope = GetDefaultMockServiceScope(mockWaveFunction);
        var initParameters = new SuperpositionInitialisationParameters
        {
            ServiceScope = mockScope.Object
        };

        await agent.Initialise(initParameters);

        mockWaveFunction.Verify(w => w.Initialise(agent, initParameters.SuperpositionMappings), Times.Once);

        agent.Scope.Should().BeSameAs(mockScope.Object);
    }

    [Test]
    public async Task Collapse_WhenTheLocalWaveFunctionReturnsNull_ReturnsNullToCaller()
    {
        var agent = await GetInitialisedSuperpositionAgent();
        var result = await agent.Collapse(new NanoCollapseParameters
        {
            NanoTypeId = Guid.NewGuid()
        });

        result.Should().BeNull();
    }

    [Test]
    public async Task Collapse_WhenTheLocalWaveFunctionDoesNNull_ReturnsTheAgentAndRegistersIt()
    {
        var collapseId = Guid.NewGuid();
        var mockWaveFunction = new Mock<ISuperpositionNanoWaveFunction>();
        var mockNanoTypeManager = new Mock<INanoTypeManager>();
        var mockCollapseAgent = new Mock<INanoAgent>();

        mockWaveFunction
            .Setup(w => w.Collapse(It.Is<NanoCollapseParameters>(p => p.NanoTypeId == collapseId)))
            .ReturnsAsync(mockCollapseAgent.Object);

        var agent = await GetInitialisedSuperpositionAgent(mockWaveFunction, mockNanoTypeManager);
        var result = await agent.Collapse(new NanoCollapseParameters
        {
            NanoTypeId = collapseId
        });

        result.Should().NotBeNull();
        result.Should().BeSameAs(mockCollapseAgent.Object);

        mockNanoTypeManager.Verify(n => n.RegisterNanoType(mockCollapseAgent.Object), Times.Once);
    }

    [Test]
    public async Task Recollapse_WhenLocalFunctionIsCorrect_InvokesTheRecollapseOfTheLocalFunction()
    {
        var resultId = Guid.NewGuid();
        var mockWaveFunction = new Mock<ISuperpositionNanoWaveFunction>();

        mockWaveFunction
            .Setup(w => w.Recollapse(It.IsAny<NanoRecollapseParameters>()))
            .ReturnsAsync(resultId);

        var agent = await GetInitialisedSuperpositionAgent(mockWaveFunction);
        var recollapseParameters = new NanoRecollapseParameters
        {
            SourceTeraId = Guid.NewGuid(),
            TeraIds = new[]
            {
                Guid.NewGuid()
            }
        };
        var result = await agent.Recollapse(recollapseParameters);

        result.Should().Be(resultId);

        mockWaveFunction.Verify(w => w.Recollapse(recollapseParameters));
    }

    [Test]
    public async Task Recollapse_WhenLocalFunctionIsNotISuperpositionWaveNanoFunction_ReturnsNull()
    {
        var agent = GetSuperpositionAgent();
        var result = await agent.Recollapse(new NanoRecollapseParameters());

        result.Should().BeNull();
    }

    [Test]
    public async Task Sync_DependencyTerminationNano_InvokesTheITeraAgentNanoTypeDependencyService()
    {
        var mockNanoTypeDependencyService = new Mock<ITeraAgentNanoTypeDependencyService>();
        var agent = await GetInitialisedSuperpositionAgent(mockNanoTypeDependencyService: mockNanoTypeDependencyService);
        var parameters = new NanoTypeDependencyTerminationParameters
        {
            NanoType = new SplinterId
            {
                Guid = Guid.NewGuid()
            },
            TeraId = Guid.NewGuid()
        };

        await agent.Sync(parameters);

        mockNanoTypeDependencyService.Verify(n => n.DecrementTeraAgentNanoTypeDependencies(
            parameters.TeraId,
            parameters.NanoType,
            1), Times.Once);
    }

    [Test]
    public async Task Sync_Recollapse_InvokesTheINanoTypeRecollapseOperationService()
    {
        var mockRecollapseOperationService = new Mock<INanoTypeRecollapseOperationService>();
        var agent = await GetInitialisedSuperpositionAgent(mockRecollapseOperationService: mockRecollapseOperationService);
        var parameters = new NanoRecollapseOperationParameters();

        await agent.Sync(parameters);

        mockRecollapseOperationService.Verify(r => r.Sync(parameters), Times.Once);
    }

    private static Mock<IServiceScope> GetDefaultMockServiceScope(
        Mock<ISuperpositionNanoWaveFunction>? mockSuperpositionNanoWaveFunction = null,
        Mock<INanoTypeManager>? mockNanoTypeManager = null,
        Mock<ITeraAgentNanoTypeDependencyService>? mockNanoTypeDependencyService = null,
        Mock<INanoTypeRecollapseOperationService>? mockRecollapseOperationService = null)
    {
        var result = new Mock<IServiceScope>();

        mockSuperpositionNanoWaveFunction ??= new Mock<ISuperpositionNanoWaveFunction>();
        mockNanoTypeManager ??= new Mock<INanoTypeManager>();
        mockNanoTypeDependencyService ??= new Mock<ITeraAgentNanoTypeDependencyService>();
        mockRecollapseOperationService ??= new Mock<INanoTypeRecollapseOperationService>();

        result
            .Setup(s => s.Start())
            .ReturnsAsync(result.Object);

        result
            .Setup(s => s.Resolve<ISuperpositionNanoWaveFunction>())
            .ReturnsAsync(mockSuperpositionNanoWaveFunction.Object);

        result
            .Setup(s => s.Resolve<INanoTypeManager>())
            .ReturnsAsync(mockNanoTypeManager.Object);

        result
            .Setup(s => s.Resolve<ITeraAgentNanoTypeDependencyService>())
            .ReturnsAsync(mockNanoTypeDependencyService.Object);

        result
            .Setup(s => s.Resolve<INanoTypeRecollapseOperationService>())
            .ReturnsAsync(mockRecollapseOperationService.Object);

        return result;
    }

    private static async Task<ISuperpositionAgent> GetInitialisedSuperpositionAgent(
        Mock<ISuperpositionNanoWaveFunction>? mockSuperpositionNanoWaveFunction = null,
        Mock<INanoTypeManager>? mockNanoTypeManager = null,
        Mock<ITeraAgentNanoTypeDependencyService>? mockNanoTypeDependencyService = null,
        Mock<INanoTypeRecollapseOperationService>? mockRecollapseOperationService = null)
    {
        var agent = GetSuperpositionAgent();
        var mockScope = GetDefaultMockServiceScope(
            mockSuperpositionNanoWaveFunction, 
            mockNanoTypeManager,
            mockNanoTypeDependencyService,
            mockRecollapseOperationService);
        var initParameters = new SuperpositionInitialisationParameters
        {
            ServiceScope = mockScope.Object
        };

        await agent.Initialise(initParameters);

        return agent;
    }

    private static ISuperpositionAgent GetSuperpositionAgent()
    {
        return new SuperpositionAgent();
    }
}