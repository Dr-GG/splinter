using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Registration;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Containers;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Tests.AgentsTests.TeraAgentsTests;

public class TeraAgentTests
{
    private Mock<IServiceScope> _mockServiceScope = new();
    private Mock<ITeraKnowledgeAgent> _mockTeraKnowledgeAgent = new();
    private Mock<ISuperpositionAgent> _mockSuperpositionAgent = new();
    private Mock<ITeraRegistryAgent> _mockRegistryAgent = new();
    private Mock<ITeraAgentContainer> _mockTeraAgentContainer = new();

    [SetUp]
    public void SetupSplinterEnvironment()
    {
        SetupBasicScope();

        _mockSuperpositionAgent = GetDefaultMockSuperpositionAgent();
        _mockTeraKnowledgeAgent = new Mock<ITeraKnowledgeAgent>();
        _mockRegistryAgent = new Mock<ITeraRegistryAgent>();
        _mockTeraAgentContainer = new Mock<ITeraAgentContainer>();

        SplinterEnvironment.SuperpositionAgent = _mockSuperpositionAgent.Object;
        SplinterEnvironment.TeraRegistryAgent = _mockRegistryAgent.Object;
        SplinterEnvironment.TeraAgentContainer = _mockTeraAgentContainer.Object;
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Initialised;
    }

    [TearDown]
    public void DisposeSplinterEnvironment()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Uninitialised;
    }

    [Test]
    public void HasNoTeraParent_ReturnsTrue()
    {
        var teraAgent = GetTeraAgent();

        teraAgent.HasNoTeraParent.Should().BeTrue();
    }

    [Test]
    public void HasTeraParent_ReturnsFalse()
    {
        var teraAgent = GetTeraAgent();

        teraAgent.HasTeraParent.Should().BeFalse();
    }

    [Test]
    public async Task NanoTableProperties_WhenScopeProvidesNoNanoTable_ReturnsFalse()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        teraAgent.HasNanoTable.Should().BeFalse();
        teraAgent.HasNoNanoTable.Should().BeTrue();
        _mockServiceScope.Verify(x => x.Resolve<INanoTable>(), Times.Once);

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => teraAgent.NanoTable.Should().NotBeNull());
    }

    [Test]
    public async Task NanoTableProperties_WhenRegisterNanoTableIsFalse_ReturnsFalse()
    {
        SetupNanoTableScope();

        var teraAgent = await GetInitialisedTeraAgent(new NanoInitialisationParameters
        {
            RegisterNanoTable = false
        });

        teraAgent.HasNanoTable.Should().BeFalse();
        teraAgent.HasNoNanoTable.Should().BeTrue();
        _mockServiceScope.Verify(x => x.Resolve<INanoTable>(), Times.Never);

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => teraAgent.NanoTable.Should().NotBeNull());
    }

    [Test]
    public async Task NanoTableProperties_WhenScopeProvidesANanoTable_ReturnsTrue()
    {
        SetupNanoTableScope();

        var teraAgent = await GetInitialisedTeraAgent();
        
        teraAgent.HasNanoTable.Should().BeTrue();
        teraAgent.HasNoNanoTable.Should().BeFalse();
        teraAgent.NanoTable.Should().NotBeNull();
        _mockServiceScope.Verify(x => x.Resolve<INanoTable>(), Times.Once);
    }

    [Test]
    public async Task Knowledge_WhenInitParametersDoNotRegisterNanoReferences_ThrowsAnException()
    {
        var teraAgent = await GetInitialisedTeraAgent(new NanoInitialisationParameters
        { 
            RegisterNanoReferences = false
        });

        Assert.Throws<NanoTypeNotInitialiseException<ITeraKnowledgeAgent>>(() => teraAgent.Knowledge.Should().NotBeNull());
    }

    [Test]
    public async Task Knowledge_WhenHasKnowledgePropertyIsFalse_ThrowsAnException()
    {
        var teraAgent = GetTeraAgent();

        ((UnitTestTeraAgent)teraAgent).OverrideHasKnowledge = false;

        await teraAgent.Initialise(new NanoInitialisationParameters());

        Assert.Throws<NanoTypeNotInitialiseException<ITeraKnowledgeAgent>>(() => teraAgent.Knowledge.Should().NotBeNull());
    }

    [Test]
    public async Task Initialise_WhenInvoked_InvokesCollapseOfKnowledgeWithDefaultTeraId()
    {
        SetupCollapseTeraAgentKnowledge();
        SetupNanoTableScope();

        await GetInitialisedTeraAgent();

        _mockSuperpositionAgent.Verify(s => 
            s.Collapse(
                It.Is<NanoCollapseParameters>(p => p.NanoTypeId == SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId.Guid)), 
            Times.Once);
    }

    [Test]
    public async Task Initialise_WhenOverrideTheTeraKnowledgeAgentNanoTypeId_InvokesCollapseOfKnowledgeWithOverridenValue()
    {
        SetupCollapseTeraAgentKnowledge();
        SetupNanoTableScope();

        var teraAgent = GetTeraAgent();
        var nanoTypeId = Guid.NewGuid();

        ((UnitTestTeraAgent) teraAgent).OverrideTeraAgentNanoTypeId = new SplinterId {Guid = nanoTypeId};

        await teraAgent.Initialise(new NanoInitialisationParameters());

        _mockSuperpositionAgent.Verify(s =>
                s.Collapse(
                    It.Is<NanoCollapseParameters>(p => p.NanoTypeId == nanoTypeId)),
            Times.Once);
    }

    [Test]
    public async Task Knowledge_WhenSuperpositionCollapseProvidesNoKnowledge_ThrowsAnException()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        Assert.Throws<InvalidNanoInstanceException>(() => teraAgent.Knowledge.Should().NotBeNull());
    }

    [Test]
    public async Task Knowledge_WhenSuperpositionCollapseProvidesKnowledge_ReturnsTheNanoReference()
    {
        SetupCollapseTeraAgentKnowledge();

        var teraAgent = await GetInitialisedTeraAgent();

        teraAgent.Knowledge.Should().NotBeNull();
    }

    [Test]
    public async Task TeraParent_EqualsSelf()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        teraAgent.TeraParent.Should().BeSameAs(teraAgent);
    }

    [Test]
    public async Task Initialise_WhenInitParametersSetToNotRegister_DoesNotRegister()
    {
        var teraAgent = await GetInitialisedTeraAgent(new NanoInitialisationParameters
        {
            Register = false
        });

        teraAgent.TeraId.Should().BeEmpty();
        _mockRegistryAgent.Verify(x => x.Register(It.IsAny<TeraAgentRegistrationParameters>()), Times.Never);
    }

    [Test]
    public async Task Initialise_WhenInitParametersHasTeraIdSet_CallRegistrationWithProvidedTeraId()
    {
        var teraId = Guid.NewGuid();

        _mockRegistryAgent
            .Setup(r => r.Register(It.IsAny<TeraAgentRegistrationParameters>()))
            .ReturnsAsync(teraId);

        var teraAgent = await GetInitialisedTeraAgent(new NanoInitialisationParameters
        {
            TeraId = teraId
        });

        teraAgent.TeraId.Should().Be(teraId);
        _mockRegistryAgent.Verify(x => x.Register(
            It.Is<TeraAgentRegistrationParameters>(
                parameters => parameters.TeraId == teraId)), Times.Once);
    }

    [Test]
    public async Task Initialise_WhenRegisterInContainerSetToTrue_RegistersInContainer()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        _mockTeraAgentContainer.Verify(t => t.Register(teraAgent), Times.Once);
    }

    [Test]
    public async Task Initialise_WhenRegisterInContainerSetToFalse_DoesNotRegisterInContainer()
    {
        var teraAgent = GetTeraAgent();

        ((UnitTestTeraAgent) teraAgent).OverrideRegisterInContainer = false;

        await teraAgent.Initialise(new NanoInitialisationParameters());

        _mockTeraAgentContainer.Verify(t => t.Register(teraAgent), Times.Never);
    }

    [Test]
    public async Task RegisterSelf_WhenInvoked_CallsTheRegistryAgentWithTheAppropriateInstanceId()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        _mockRegistryAgent
            .Verify(r => r.Register(It.Is<TeraAgentRegistrationParameters>(
                    p => p.NanoInstanceId == teraAgent.InstanceId)), Times.Once);
    }

    [Test]
    public async Task RegisterSelf_WhenInvokedAfterInitialisation_DoesNotReInitialiseVariousDependencies()
    {
        SetupCollapseTeraAgentKnowledge();
        SetupNanoTableScope();

        var teraAgent = await GetInitialisedTeraAgent();

        await teraAgent.RegisterSelf(new TeraAgentRegistrationParameters());

        teraAgent.NanoTable.Should().NotBeNull();
        teraAgent.Knowledge.Should().NotBeNull();
        
        _mockServiceScope.Verify(s => s.Resolve<INanoTable>(), Times.Once);
        _mockSuperpositionAgent.Verify(s => s.Collapse(It.IsAny<NanoCollapseParameters>()), Times.Once);
    }

    [Test]
    public async Task RegisterSelf_WhenInvokedWithoutInitialisation_RegistersCrucialDependencies()
    {
        SetupCollapseTeraAgentKnowledge();
        SetupNanoTableScope();

        var teraAgent = GetTeraAgent();

        ((UnitTestTeraAgent)teraAgent).OverrideScope(_mockServiceScope.Object);

        await teraAgent.RegisterSelf(new TeraAgentRegistrationParameters());

        teraAgent.NanoTable.Should().NotBeNull();
        teraAgent.Knowledge.Should().NotBeNull();

        _mockServiceScope.Verify(s => s.Resolve<INanoTable>(), Times.Once);
        _mockSuperpositionAgent.Verify(s => s.Collapse(It.IsAny<NanoCollapseParameters>()), Times.Once);
    }

    [Test]
    public async Task Execute_WhenNotInitialised_DoesNotThrowAnErrorAndDoesNotInvokeTheTeraAgentKnowledge()
    {
        var teraAgent = GetTeraAgent();

        await teraAgent.Execute(new TeraAgentExecutionParameters());

        _mockTeraKnowledgeAgent.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Execute_WhenInitialisedButNoTeraAgentKnowledgeWasCollapsed_DoesNotThrowAnErrorAndDoesNotInvokeTheTeraAgentKnowledge()
    {
        var teraAgent = await GetInitialisedTeraAgent();

        await teraAgent.Execute(new TeraAgentExecutionParameters());

        _mockTeraKnowledgeAgent.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Execute_WhenInitialisedWithTeraAgentKnowledge_ExecutesTheTeraAgentKnowledge()
    {
        SetupCollapseTeraAgentKnowledge();

        var teraAgent = await GetInitialisedTeraAgent();
        var executionParameters = new TeraAgentExecutionParameters();

        await teraAgent.Execute(executionParameters);

        _mockTeraKnowledgeAgent.Verify(t => t.Execute(executionParameters), Times.Once);
    }

    [Test]
    public async Task Collapse_WhenProvidedWithANonEmptyWaveFunction_ReturnsTheCollapsedNanoAgent()
    {
        var teraAgent = await GetInitialisedTeraAgent();
        var mockWaveFunction = new Mock<INanoWaveFunction>();

        mockWaveFunction
            .Setup(s => s.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(_mockTeraKnowledgeAgent.Object);

        ((UnitTestTeraAgent)teraAgent).OverrideWaveFunction(mockWaveFunction.Object);

        var agent = await teraAgent.Collapse(new NanoCollapseParameters());

        agent.Should().NotBeNull();
        agent.Should().BeSameAs(_mockTeraKnowledgeAgent.Object);
    }

    [Test]
    public async Task Collapse_WhenSplinterEnvironmentIsNotInitialised_ReturnsNull()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Uninitialised;

        var teraAgent = await GetInitialisedTeraAgent();
        var agent = await teraAgent.Collapse(new NanoCollapseParameters());

        agent.Should().BeNull();
    }

    [Test]
    public async Task Collapse_WhenSuperpositionAgentDoesNotCollapse_ReturnsNull()
    {
        var teraAgent = await GetInitialisedTeraAgent();
        var agent = await teraAgent.Collapse(new NanoCollapseParameters());

        agent.Should().BeNull();
    }

    [Test]
    public async Task Collapse_WhenSuperpositionAgentDoesCollapse_ReturnsNotNull()
    {
        SetupCollapseTeraAgentKnowledge();

        var teraAgent = await GetInitialisedTeraAgent();
        var collapseParameters = new NanoCollapseParameters();
        var agent = await teraAgent.Collapse(collapseParameters);

        agent.Should().NotBeNull();
        agent.Should().BeSameAs(_mockTeraKnowledgeAgent.Object);

        _mockSuperpositionAgent.Verify(s => s.Collapse(collapseParameters));
    }

    [Test]
    public async Task Terminate_WhenNotInitialised_StillTerminates()
    {
        var teraAgent = GetTeraAgent();

        ((UnitTestTeraAgent)teraAgent).OverrideScope(_mockServiceScope.Object);

        await teraAgent.Terminate(new NanoTerminationParameters());

        AssertTeraAgentIsDisposed(teraAgent);
        Assert.Throws<NanoTypeNotInitialiseException<ITeraKnowledgeAgent>>(() => teraAgent.Knowledge.Should().BeNull());
    }

    [Test]
    public async Task Terminate_WhenInitialised_Terminates()
    {
        SetupCollapseTeraAgentKnowledge();
        SetupNanoTableScope();

        var teraAgent = await GetInitialisedTeraAgent();

        ((UnitTestTeraAgent)teraAgent).OverrideScope(_mockServiceScope.Object);

        await teraAgent.Terminate(new NanoTerminationParameters());

        AssertTeraAgentIsDisposed(teraAgent);
        Assert.Throws<InvalidNanoInstanceException>(() => teraAgent.Knowledge.Should().BeNull());
    }

    private void AssertTeraAgentIsDisposed(ITeraAgent teraAgent)
    {
        _mockServiceScope.Verify(s => s.DisposeAsync());
        _mockTeraAgentContainer.Verify(c => c.Deregister(teraAgent), Times.Once);
        _mockRegistryAgent.Verify(c => c.Deregister(It.Is<TeraAgentDeregistrationParameters>(p => p.TeraId == teraAgent.TeraId)), Times.Once);
    }

    private static async Task<ITeraAgent> GetInitialisedTeraAgent(NanoInitialisationParameters? parameters = null)
    {
        var teraAgent = GetTeraAgent();

        parameters ??= new NanoInitialisationParameters();

        await teraAgent.Initialise(parameters);

        return teraAgent;
    }

    private static ITeraAgent GetTeraAgent()
    {
        return new UnitTestTeraAgent();
    }

    private void SetupBasicScope()
    {
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScope
            .Setup(s => s.Start())
            .ReturnsAsync(_mockServiceScope.Object);
    }

    private void SetupNanoTableScope()
    {
        _mockServiceScope
            .Setup(s => s.Resolve<INanoTable>())
            .ReturnsAsync(new Mock<INanoTable>().Object);
    }

    private void SetupCollapseTeraAgentKnowledge()
    {
        _mockSuperpositionAgent
            .Setup(m => m.Collapse(It.IsAny<NanoCollapseParameters>()))
            .ReturnsAsync(_mockTeraKnowledgeAgent.Object);
    }

    private Mock<ISuperpositionAgent> GetDefaultMockSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        result
            .SetupGet(s => s.Scope)
            .Returns(_mockServiceScope.Object);

        return result;
    }
}
