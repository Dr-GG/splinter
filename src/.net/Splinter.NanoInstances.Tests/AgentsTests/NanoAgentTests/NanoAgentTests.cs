using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Tests.Agents;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Tests.AgentsTests.NanoAgentTests;

[TestFixture]
public class NanoAgentTests
{
    private Mock<IServiceScope> _mockServiceScope = new();
    private Mock<ISuperpositionAgent> _mockSuperpositionAgent = new();
    private Mock<ITeraPlatformAgent> _mockTeraPlatformAgent = new();
    private Mock<ITeraRegistryAgent> _mockTeraRegistryAgent = new();
    private Mock<ITeraMessageAgent> _mockTeraMessageAgent = new();

    private static readonly Guid DefaultTeraParentId = Guid.Parse("F839280A-6AF7-45E2-9CF6-A1CA633031EE");

    [SetUp]
    public void InitialiseSplinterEnvironment()
    {
        _mockServiceScope = new Mock<IServiceScope>();
        _mockSuperpositionAgent = new Mock<ISuperpositionAgent>();
        _mockTeraMessageAgent = new Mock<ITeraMessageAgent>();
        _mockTeraPlatformAgent = new Mock<ITeraPlatformAgent>();
        _mockTeraRegistryAgent = new Mock<ITeraRegistryAgent>();

        SplinterEnvironment.SuperpositionAgent = _mockSuperpositionAgent.Object;
        SplinterEnvironment.TeraMessageAgent = _mockTeraMessageAgent.Object;
        SplinterEnvironment.TeraPlatformAgent = _mockTeraPlatformAgent.Object;
        SplinterEnvironment.TeraRegistryAgent = _mockTeraRegistryAgent.Object;

        SplinterEnvironment.Status = SplinterEnvironmentStatus.Initialised;

        SetupMockScope();
    }

    [TearDown]
    public void TerminateSplinterEnvironment()
    {
        SplinterEnvironment.Status = SplinterEnvironmentStatus.Disposed;
    }

    [Test]
    public void HasTeraParent_WhenNoTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        result.HasTeraParent.Should().BeFalse();
    }

    [Test]
    public void HasNoTeraParent_WhenNoTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent();

        result.HasNoTeraParent.Should().BeTrue();
    }

    [Test]
    public void HasTeraParent_WhenTeraParentIsSet_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        result.HasTeraParent.Should().BeTrue();
    }

    [Test]
    public void HasNoTeraParent_WhenTeraParentIsSet_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        result.HasNoTeraParent.Should().BeFalse();
    }

    [Test]
    public void TeraParent_WhenNoTeraParentIsSet_ThrowsException()
    {
        var result = GetNanoAgent();

        Assert.Throws<TeraParentNotInitialisedException>(() => _ = result.TeraParent);
    }

    [Test]
    public void TeraParent_WhenTeraParentIsSet_ThrowsNoException()
    {
        var result = GetNanoAgent(true);

        Assert.DoesNotThrow(() => _ = result.TeraParent);
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsNotSet_ReturnsTrue()
    {
        var result = GetNanoAgent();

        result.HasNoNanoTable.Should().BeTrue();
    }

    [Test]
    public void HasNanoTable_WhenParentIsNotSet_ReturnsFalse()
    {
        var result = GetNanoAgent();

        result.HasNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsTrue()
    {
        var result = GetNanoAgent(true);

        result.HasNoNanoTable.Should().BeTrue();
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNoNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true);

        result.HasNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNoNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);

        result.HasNoNanoTable.Should().BeFalse();
    }

    [Test]
    public void HasNanoTable_WhenParentIsSetAndParentHasNanoTable_ReturnsFalse()
    {
        var result = GetNanoAgent(true, true);

        result.HasNanoTable.Should().BeTrue();
    }
        
    [Test]
    public void NanoTable_WhenNoParentIsSet_ThrowsException()
    {
        var result = GetNanoAgent();

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => _ = result.NanoTable);
    }

    [Test]
    public void NanoTable_WhenParentIsSetWithNoNanoTable_ThrowsException()
    {
        var result = GetNanoAgent(true);

        Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => _ = result.NanoTable);
    }

    [Test]
    public void NanoTable_WhenParentIsSetWithNanoTable_ThrowsNoException()
    {
        var result = GetNanoAgent(true, true);

        Assert.DoesNotThrow(() => _ = result.NanoTable);
    }

    [Test]
    public void HolonType_IsAlwaysNano()
    {
        var result = GetNanoAgent();

        result.HolonType.Should().Be(HolonType.Nano);
    }

    [Test]
    public void TeraParent_WhenNotSet_ThrowsAnException()
    {
        var result = GetNanoAgent();

        Assert.Throws<TeraParentNotInitialisedException>(() => _ = result.TeraParent);
    }

    [TestCase(true, true, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, true)]
    [TestCase(false, false, true)]
    public void NanoTable_WhenMatchingCertainParameters_ReturnsTheExceptedResult(
        bool hasTeraParent,
        bool teraParentHasNanoTable,
        bool throwsException)
    {
        var agent = GetNanoAgent(hasTeraParent, teraParentHasNanoTable);

        if (throwsException)
        {
            Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => _ = agent.NanoTable);
        }
        else
        {
            Assert.DoesNotThrow(() => _ = agent.NanoTable);
        }
    }

    [Test]
    public void SplinterEnvironmentProperties_WhenAccessed_ReturnsTheSetVariables()
    {
        var agent = GetNanoAgent();

        agent.SuperpositionAgent.Should().BeSameAs(_mockSuperpositionAgent.Object);
        agent.TeraPlatformAgent.Should().BeSameAs(_mockTeraPlatformAgent.Object);
        agent.TeraRegistryAgent.Should().BeSameAs(_mockTeraRegistryAgent.Object);
        agent.TeraMessageAgent.Should().BeSameAs(_mockTeraMessageAgent.Object);
    }

    [Test]
    public async Task Initialise_WhenInvoked_SetsTheScope()
    {
        var mockScope = new Mock<IServiceScope>();
        var parameters = new NanoInitialisationParameters {ServiceScope = mockScope.Object};
        var agent = await GetInitialisedNanoAgent(parameters);

        agent.Scope.Should().BeSameAs(mockScope.Object);
    }

    [Test]
    public async Task Initialise_WhenInvoked_InitialiseTheNanoReferences()
    {
        var agent = await GetInitialisedNanoAgent();

        ((UnitTestNanoAgentWithSplinterIds)agent).InvokedInternalInitialiseNanoReferences.Should().BeTrue();
    }

    [Test]
    public async Task Initialise_WhenInvokedWithParametersNotToRegisterNanoReferences_DoesNotInitialiseTheNanoReferences()
    {
        var agent = await GetInitialisedNanoAgent(new NanoInitialisationParameters
        {
            RegisterNanoReferences = false
        });

        ((UnitTestNanoAgentWithSplinterIds)agent).InvokedInternalInitialiseNanoReferences.Should().BeFalse();
    }

    [Test]
    public async Task Collapse_WhenInvokedWithoutATeraParent_DoesNotAttachTheTeraParentId()
    {
        var agent = await GetInitialisedNanoAgent();
        var collapseParameters = new NanoCollapseParameters
        {
            NanoTypeId = Guid.NewGuid(),
        };

        await agent.Collapse(collapseParameters);

        _mockSuperpositionAgent.Verify(
            s => s.Collapse(
                It.Is<NanoCollapseParameters>(p => 
                    p.TeraAgentId == null && 
                    p.NanoTypeId == collapseParameters.NanoTypeId)));
    }

    [Test]
    public async Task Collapse_WhenInvokedWithATeraParent_AttachesTheTeraParentId()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var collapseParameters = new NanoCollapseParameters
        {
            NanoTypeId = Guid.NewGuid(),
        };

        await agent.Collapse(collapseParameters);

        _mockSuperpositionAgent.Verify(
            s => s.Collapse(
                It.Is<NanoCollapseParameters>(p =>
                    p.TeraAgentId == DefaultTeraParentId &&
                    p.NanoTypeId == collapseParameters.NanoTypeId)));
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunction_ReturnsTheCollapsedNanoAgent()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters());

        result.Should().NotBeNull();
        result.Should().BeSameAs(mockCollapsedAgent.Object);
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunctionAndIsANanoType_AttachesTheParent()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        await agent.Collapse(new NanoCollapseParameters());
        
        mockCollapsedAgent.VerifySet(s => s.TeraParent = agent.TeraParent);
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunction_InitialisesTheNanoAgent()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters());

        result.Should().NotBeNull();
        
        mockCollapsedAgent.Verify(s => s.Initialise(It.IsAny<NanoInitialisationParameters>()), Times.Once());
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunctionAndHasATeraParent_InitialisesTheNanoAgentWithTheTeraParentId()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters());

        result.Should().NotBeNull();

        mockCollapsedAgent.Verify(
            s => s.Initialise(It.Is<NanoInitialisationParameters>(
                p => p.TeraId == agent.TeraParent.TeraId)), Times.Once());
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunctionAndHasNoTeraParent_InitialisesTheNanoAgentWithoutTheTeraParentId()
    {
        var agent = await GetInitialisedNanoAgent(null, false, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters());

        result.Should().NotBeNull();

        mockCollapsedAgent.Verify(
            s => s.Initialise(It.Is<NanoInitialisationParameters>(
                p => p.TeraId == null)), Times.Once());
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunctionAndInitialiseIsFalse_DoesNotInitialiseTheCollapsedAgent()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters
        {
            Initialise = false
        });

        result.Should().NotBeNull();
        mockCollapsedAgent.Verify(s => s.Initialise(It.IsAny<NanoInitialisationParameters>()), Times.Never());
    }

    [Test]
    public async Task Collapse_WhenCollapsingANanoAgentUsingTheLocalWaveFunctionAndProvidingCustomInitialisationParameters_UsesTheCustomInitialisationParameters()
    {
        var agent = await GetInitialisedNanoAgent(null, true, true);
        var mockCollapsedAgent = GetMockCollapsedNanoAgent();
        var mockWaveFunction = GetMockNanoWaveFunction(mockCollapsedAgent.Object);
        var customInitialisation = new NanoInitialisationParameters();

        ((UnitTestNanoAgentWithSplinterIds)agent).OverrideNanoWaveFunction(mockWaveFunction.Object);

        var result = await agent.Collapse(new NanoCollapseParameters
        {
            Initialise = false
        });

        result.Should().NotBeNull();
        mockCollapsedAgent.Verify(s => s.Initialise(customInitialisation), Times.Never());
    }

    // Test tera.

    private static Mock<INanoWaveFunction> GetMockNanoWaveFunction(INanoAgent collapsedAgent)
    {
        var result = new Mock<INanoWaveFunction>();

        result.Setup(w => 
            w.Collapse(It.IsAny<NanoCollapseParameters>())).ReturnsAsync(collapsedAgent);

        return result;
    }

    private static Mock<INanoAgent> GetMockCollapsedNanoAgent(HolonType holonType = HolonType.Nano)
    {
        var result = new Mock<INanoAgent>();

        result.SetupGet(n => n.HolonType).Returns(holonType);

        return result;
    }

    private async Task<INanoAgent> GetInitialisedNanoAgent(
        NanoInitialisationParameters? parameters = null,
        bool setParent = false,
        bool parentHasNanoTable = false)
    {
        var result = GetNanoAgent(setParent, parentHasNanoTable);

        parameters ??= new NanoInitialisationParameters
        {
            ServiceScope = _mockServiceScope.Object
        };

        await result.Initialise(parameters);

        return result;
    }

    private static INanoAgent GetNanoAgent(
        bool setParent = false,
        bool parentHasNanoTable = false)
    {
        var result = new UnitTestNanoAgentWithSplinterIds();

        if (!setParent)
        {
            return result;
        }

        result.TeraParent = GetTeraParent(parentHasNanoTable);

        return result;
    }

    private static ITeraAgent GetTeraParent(bool hasNanoTable)
    {
        var result = new Mock<ITeraAgent>();

        result.Setup(t => t.HasNanoTable).Returns(hasNanoTable);
        result.Setup(t => t.HasNoNanoTable).Returns(!hasNanoTable);
        result.Setup(t => t.TeraId).Returns(DefaultTeraParentId);

        if (hasNanoTable)
        {
            result.Setup(t => t.NanoTable).Returns(new Mock<INanoTable>().Object);
        }

        return result.Object;
    }

    private void SetupMockScope()
    {
        _mockServiceScope
            .Setup(s => s.Start())
            .ReturnsAsync(_mockServiceScope.Object);
    }
}