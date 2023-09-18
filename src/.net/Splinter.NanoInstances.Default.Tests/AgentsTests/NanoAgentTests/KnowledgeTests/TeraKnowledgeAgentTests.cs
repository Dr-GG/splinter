using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Agents.NanoAgents.Knowledge;
using Splinter.NanoInstances.Default.Tests.Agents.Knowledge;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.Tests.AgentsTests.NanoAgentTests.KnowledgeTests;

public class TeraKnowledgeAgentTests
{
    private static readonly Guid DefaultTeraParentId = Guid.NewGuid();

    private class MessageContext
    {
        public int CurrentIndex { get; set; }

        public IEnumerable<TeraMessage> Messages { get; set; } = Array.Empty<TeraMessage>();
    }

    private static TeraMessage CustomTeraMessage => new()
    {
        Id = DateTime.UtcNow.Ticks,
        Message = "Custom Tera Message",
        Code = 100,
        SourceTeraId = Guid.NewGuid(),
        BatchId = Guid.NewGuid(),
        LoggedTimestamp = DateTime.UtcNow
    };

    private static TeraMessage RecollapseTeraMessageWithData => new()
    {
        Id = DateTime.UtcNow.Ticks,
        Message = JsonSerializer.Serialize(new RecollapseTeraMessageData
        {
            NanoTypeId = Guid.NewGuid(),
            NanoTypeRecollapseOperationId = Guid.NewGuid(),
        }),
        Code = TeraMessageCodeConstants.Recollapse,
        SourceTeraId = Guid.NewGuid(),
        BatchId = Guid.NewGuid(),
        LoggedTimestamp = DateTime.UtcNow
    };

    private static TeraMessage RecollapseTeraMessageWithoutData => new()
    {
        Id = DateTime.UtcNow.Ticks,
        Code = TeraMessageCodeConstants.Recollapse,
        SourceTeraId = Guid.NewGuid(),
        BatchId = Guid.NewGuid(),
        LoggedTimestamp = DateTime.UtcNow
    };

    [Test]
    public void NanoTypeId_ShouldBeEqualToTeraDefaultKnowledgeNanoTypeId()
    {
        var knowledgeAgent = GetKnowledgeAgent();
        var nanoTypeId = knowledgeAgent.TypeId;
        
        nanoTypeId.Should().Be(SplinterIdConstants.TeraDefaultKnowledgeNanoTypeId);
    }

    [Test]
    public void NanoInstanceId_ShouldBeEqualToTeraDefaultKnowledgeNanoTypeId()
    {
        var knowledgeAgent = GetKnowledgeAgent();
        var nanoTypeId = knowledgeAgent.InstanceId;

        nanoTypeId.Should().Be(TeraKnowledgeAgent.NanoInstanceId);
    }

    [Test]
    public async Task Initialise_WhenInitialiseWithoutParent_DoesNotResolveTheTeraMessageQueue()
    {
        var mockScope = GetDefaultMockServiceScope();
        
        await GetInitialisedKnowledgeAgent(false, serviceScope: mockScope.Object);

        mockScope.Verify(s => s.Resolve<ITeraMessageQueue>(), Times.Never);
    }

    [Test]
    public async Task Initialise_WhenInitialisedWithAParent_ResolvesTheTeraMessageQueue()
    {
        var mockScope = GetDefaultMockServiceScope();

        await GetInitialisedKnowledgeAgent(true, serviceScope: mockScope.Object);

        mockScope.Verify(s => s.Resolve<ITeraMessageQueue>(), Times.Once);
    }

    [Test]
    public async Task Execute_WhenNoTeraMessageQueueIsPresent_DoesNotProcessTeraMessages()
    {
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue();
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object);
        var agent = await GetInitialisedKnowledgeAgent(false, serviceScope: mockScope.Object);

        await agent.Execute(new TeraAgentExecutionParameters());

        mockTeraMessageQueue.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Execute_WhenACustomTeraMessageIsProvided_ProcessesItButDoesNotRecollapseAndSyncsIt()
    {
        var message = CustomTeraMessage;
        var mockTeraMessageAgent = SetupMockTeraMessageAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object);
        var agent = await GetInitialisedKnowledgeAgent(serviceScope: mockScope.Object);

        await agent.Execute(new TeraAgentExecutionParameters());

        AssertSucceededSync(message, mockTeraMessageAgent);
    }

    [Test]
    public async Task Execute_WhenACustomTeraMessageIsProvidedAndAnErrorIsGenerated_SyncsItAsFailed()
    {
        var message = CustomTeraMessage;
        var mockTeraMessageAgent = SetupMockTeraMessageAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object);
        var agent = new ErrorKnowledgeAgent();
        
        await InitialiseKnowledgeAgent(agent, serviceScope: mockScope.Object);

        await agent.Execute(new TeraAgentExecutionParameters());

        AssertFailedSync(message, ErrorKnowledgeAgent.ErrorMessage, mockTeraMessageAgent);
    }

    [Test]
    public async Task Execute_WhenRecollapsingAndTheRecollapseIsSuccessfulAndThereIsRecollapseData_SyncsTheRecollapse()
    {
        SetupMockTeraMessageAgent();

        var message = RecollapseTeraMessageWithData;
        var mockSuperpositionAgent = SetupMockSuperpositionAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object);
        var agent = await GetInitialisedKnowledgeAgent(serviceScope: mockScope.Object);

        await agent.Execute(new TeraAgentExecutionParameters());

        var expectedRecollapseData = message.JsonMessage<RecollapseTeraMessageData>()!;
        var expectedRecollapseOperationParameters = new NanoRecollapseOperationParameters
        {
            TeraId = DefaultTeraParentId,
            IsSuccessful = true,
            NanoRecollapseOperationId = expectedRecollapseData.NanoTypeRecollapseOperationId
        };

        mockSuperpositionAgent.Verify(s => s.Sync(expectedRecollapseOperationParameters), Times.Once);
    }

    [Test]
    public async Task Execute_WhenRecollapsingAndTheRecollapseIsSuccessfulAndThereIsNoRecollapseData_SyncsNothing()
    {
        SetupMockTeraMessageAgent();

        var message = RecollapseTeraMessageWithoutData;
        var mockSuperpositionAgent = SetupMockSuperpositionAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object);
        var agent = await GetInitialisedKnowledgeAgent(serviceScope: mockScope.Object);

        await agent.Execute(new TeraAgentExecutionParameters());

        mockSuperpositionAgent.Verify(s => s.Sync(It.IsAny<NanoRecollapseOperationParameters>()), Times.Never);
    }

    [Test]
    public async Task Execute_WhenRecollapsingAndTheRecollapseFailedAndThereIsRecollapseData_SyncsTheRecollapse()
    {
        SetupMockTeraMessageAgent();

        var message = RecollapseTeraMessageWithData;
        var mockRecollapseService = new Mock<IRecollapseNanoTypeService>();
        var mockSuperpositionAgent = SetupMockSuperpositionAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object, mockRecollapseService.Object);
        var agent = await GetInitialisedKnowledgeAgent(serviceScope: mockScope.Object);

        mockRecollapseService
            .Setup(r => r.Recollapse(It.IsAny<INanoAgent>(), It.IsAny<TeraMessage>()))
            .Throws(new NotSupportedException());

        await agent.Execute(new TeraAgentExecutionParameters());

        var expectedRecollapseData = message.JsonMessage<RecollapseTeraMessageData>()!;
        var expectedRecollapseOperationParameters = new NanoRecollapseOperationParameters
        {
            TeraId = DefaultTeraParentId,
            IsSuccessful = false,
            NanoRecollapseOperationId = expectedRecollapseData.NanoTypeRecollapseOperationId
        };

        mockSuperpositionAgent.Verify(s => s.Sync(expectedRecollapseOperationParameters), Times.Once);
    }

    [Test]
    public async Task Execute_WhenRecollapsingAndTheRecollapseFailedAndThereIsNoRecollapseData_SyncsNothing()
    {
        SetupMockTeraMessageAgent();

        var message = RecollapseTeraMessageWithoutData;
        var mockRecollapseService = new Mock<IRecollapseNanoTypeService>();
        var mockSuperpositionAgent = SetupMockSuperpositionAgent();
        var mockTeraMessageQueue = GetDefaultMockTeraMessageQueue(message);
        var mockScope = GetDefaultMockServiceScope(mockTeraMessageQueue.Object, mockRecollapseService.Object);
        var agent = await GetInitialisedKnowledgeAgent(serviceScope: mockScope.Object);

        mockRecollapseService
            .Setup(r => r.Recollapse(It.IsAny<INanoAgent>(), It.IsAny<TeraMessage>()))
            .Throws(new NotSupportedException());

        await agent.Execute(new TeraAgentExecutionParameters());

        mockSuperpositionAgent.Verify(s => s.Sync(It.IsAny<NanoRecollapseOperationParameters>()), Times.Never);
    }

    private static void AssertSucceededSync(TeraMessage message, Mock<ITeraMessageAgent> mockTeraMessageAgent)
    {
        mockTeraMessageAgent.Verify(
            x => x.Sync(It.Is<TeraMessageSyncParameters>(parameters =>
                   parameters.Syncs.First().TeraMessageId == message.Id
                && parameters.Syncs.First().CompletionTimestamp <= DateTime.UtcNow 
                && parameters.Syncs.First().ErrorCode == null
                && parameters.Syncs.First().ErrorMessage.IsNullOrEmpty()
                && parameters.Syncs.First().ErrorStackTrace.IsNullOrEmpty())), Times.Once);
    }

    private static void AssertFailedSync(TeraMessage message, string expectedErrorMessage, Mock<ITeraMessageAgent> mockTeraMessageAgent)
    {
        mockTeraMessageAgent.Verify(
            x => x.Sync(It.Is<TeraMessageSyncParameters>(parameters =>
                parameters.Syncs.First().TeraMessageId == message.Id
                && parameters.Syncs.First().CompletionTimestamp <= DateTime.UtcNow
                && parameters.Syncs.First().ErrorCode == TeraMessageErrorCode.Unknown
                && parameters.Syncs.First().ErrorMessage!.Contains(expectedErrorMessage)
                && parameters.Syncs.First().ErrorStackTrace.IsNotNullOrEmpty())), Times.Once);
    }

    private static Mock<ITeraMessageQueue> GetDefaultMockTeraMessageQueue(params TeraMessage[] teraMessages)
    {
        var result = new Mock<ITeraMessageQueue>();

        if (teraMessages.IsEmpty())
        {
            return result;
        }
        
        var messageContext = new MessageContext
        {
            CurrentIndex = 0,
            Messages = teraMessages.ToList()
        };

        result
            .Setup(x => x.HasNext())
            .ReturnsAsync(() => messageContext.CurrentIndex < messageContext.Messages.Count());

        result
            .Setup(x => x.Next())
            .ReturnsAsync(() => messageContext.Messages.IsEmpty()
                    ? new TeraMessage()
                    : messageContext.Messages.ElementAt(messageContext.CurrentIndex++));

        return result;
    }

    private static Mock<ISuperpositionAgent> SetupMockSuperpositionAgent()
    {
        var result = new Mock<ISuperpositionAgent>();

        SplinterEnvironment.SuperpositionAgent = result.Object;

        return result;
    }

    private static Mock<ITeraMessageAgent> SetupMockTeraMessageAgent()
    {
        var result = new Mock<ITeraMessageAgent>();

        SplinterEnvironment.TeraMessageAgent = result.Object;

        return result;
    }

    private static ITeraMessageQueue GetDefaultTeraMessageQueue()
    {
        return GetDefaultMockTeraMessageQueue().Object;
    }

    private static Mock<IServiceScope> GetDefaultMockServiceScope(
        ITeraMessageQueue? messageQueue = null,
        IRecollapseNanoTypeService? recollapseNanoTypeService = null)
    {
        var result = new Mock<IServiceScope>();

        messageQueue ??= GetDefaultTeraMessageQueue();
        recollapseNanoTypeService ??= new Mock<IRecollapseNanoTypeService>().Object;

        result
            .Setup(x => x.Start())
            .ReturnsAsync(result.Object);

        result
            .Setup(x => x.Resolve<ITeraMessageQueue>())
            .ReturnsAsync(messageQueue);

        result
            .Setup(x => x.Resolve<IRecollapseNanoTypeService>())
            .ReturnsAsync(recollapseNanoTypeService);

        return result;
    }

    private static IServiceScope GetDefaultServiceScope()
    {
        return GetDefaultMockServiceScope().Object;
    }

    private static ITeraAgent GetDefaultTeraParent()
    {
        var mock = new Mock<ITeraAgent>();

        mock.SetupGet(t => t.TeraId).Returns(DefaultTeraParentId);

        return mock.Object;
    }

    private static async Task<ITeraKnowledgeAgent> GetInitialisedKnowledgeAgent(
        bool initialiseParent = true,
        ITeraAgent? parent = null,
        IServiceScope? serviceScope = null)
    {
        var knowledgeAgent = GetKnowledgeAgent();

        await InitialiseKnowledgeAgent(
            knowledgeAgent,
            initialiseParent,
            parent,
            serviceScope);

        return knowledgeAgent;
    }

    private static async Task InitialiseKnowledgeAgent(
        INanoAgent knowledgeAgent,
        bool initialiseParent = true,
        ITeraAgent? parent = null,
        IServiceScope? serviceScope = null)
    {
        serviceScope ??= GetDefaultServiceScope();

        var initParameters = new NanoInitialisationParameters
        {
            ServiceScope = serviceScope
        };

        if (initialiseParent)
        {
            knowledgeAgent.TeraParent = parent ?? GetDefaultTeraParent();
        }

        await knowledgeAgent.Initialise(initParameters);
    }


    private static ITeraKnowledgeAgent GetKnowledgeAgent()
    {
        return new TeraKnowledgeAgent();
    }
}
