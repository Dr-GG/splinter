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

namespace Splinter.NanoInstances.Tests.AgentsTests.TeraAgentsTests;

public class TeraAgentTests
{
    private Mock<IServiceScope> _mockServiceScope = new();
    private Mock<ISuperpositionAgent> _mockSuperpositionAgent = new();
    private Mock<ITeraRegistryAgent> _mockRegistryAgent = new();
    private Mock<ITeraAgentContainer> _mockTeraAgentContainer = new();

    [SetUp]
    public void SetupSplinterEnvironment()
    {
        SetupBasicScope();

        _mockSuperpositionAgent = GetDefaultMockSuperpositionAgent();
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

    //[Test]
    //public async Task Initialise_WhenInitParametersHasTeraIdSet_CallRegistrationWithProvidedTeraId()
    //{
    //    var teraAgent = await GetInitialisedTeraAgent(new NanoInitialisationParameters
    //    {
    //        TeraId = Guid.NewGuid()
    //    });

    //    teraAgent.TeraId.Should().BeEmpty();
    //    _mockRegistryAgent.Verify(x => x.Register(It.IsAny<TeraAgentRegistrationParameters>()), Times.Never);
    //}

    [Test]
    public async Task Initialise_GETTOTHIS()
    {
        var teraAgent = GetTeraAgent();
        var initParameters = new NanoInitialisationParameters();

        await teraAgent.Initialise(initParameters);
    }

    [Test]
    public async Task Initialise_GETTOTHIS_WHENSCOPE_NOTNULL()
    {
        var teraAgent = GetTeraAgent();
        var initParameters = new NanoInitialisationParameters();

        await teraAgent.Initialise(initParameters);
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
            .ReturnsAsync(new Mock<ITeraKnowledgeAgent>().Object);
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
