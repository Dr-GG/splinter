using System;
using System.Collections.Generic;
using System.Linq;
using Splinter.Applications.Test.Utilities;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Interfaces.Agents.NanoAgents.Knowledge;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Environment;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.Applications.Test.Tests;

public static class TeraMessageTests
{
    private const int TestBulkTeraMessages = 100;

    public static async Task Test()
    {
        TestUtilities.SectionText("TERA MESSAGING TESTS");

        await TestUtilities.ExecuteTest("Testing simple message relay between two agents", TestSimpleTeraMessageRelay);
        await TestUtilities.ExecuteTest("Testing bulk message relay between two agents", TestBulkTeraMessagesBetweenAgents);
        await TestUtilities.ExecuteTest("Testing simple message relay between multiple agents", TestMultipleTeraMessageRelay);
    }

    private static async Task TestSimpleTeraMessageRelay()
    {
        var agentSource = await GetTeraMessageTeraAgent();
        var agentDestination = await GetTeraMessageTeraAgent();

        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode1, agentDestination);
        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode2, agentDestination);
        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode3, agentDestination);
        await SendTeraMessages(agentSource, 2000, agentDestination);
        await SendTeraMessages(agentSource, 3000, agentDestination);

        await AssertPendingTeraMessages(agentSource, 0);
        await AssertPendingTeraMessages(agentDestination, 5);

        await agentSource.Execute();
        await agentDestination.Execute();

        AssertMessageCounts(agentSource, 0, 0);
        AssertMessageCounts(agentDestination, 3, 2);

        await AssertCompletedTeraMessages(agentSource, 0);
        await AssertCompletedTeraMessages(agentDestination, 5);

        await agentSource.Dispose();
        await agentDestination.Dispose();
    }

    private static async Task TestMultipleTeraMessageRelay()
    {
        var agentSource = await GetTeraMessageTeraAgent();
        var agentDestination1 = await GetTeraMessageTeraAgent();
        var agentDestination2 = await GetTeraMessageTeraAgent();
        var agentDestination3 = await GetTeraMessageTeraAgent();
        var agentDestination4 = await GetTeraMessageTeraAgent();
        var agentDestination5 = await GetTeraMessageTeraAgent();

        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode1, 
            agentDestination1, agentDestination2, agentDestination3, agentDestination4, agentDestination5);
        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode2,
            agentDestination1, agentDestination2, agentDestination3, agentDestination4);
        await SendTeraMessages(agentSource, TeraMessageCodeConstants.SupportedTeraMessageCode3,
            agentDestination1, agentDestination2, agentDestination3);
        await SendTeraMessages(agentSource, 1000, agentDestination1, agentDestination2);
        await SendTeraMessages(agentSource, 2000, agentDestination1);

        await AssertPendingTeraMessages(agentSource, 0);
        await AssertPendingTeraMessages(agentDestination1, 5);
        await AssertPendingTeraMessages(agentDestination2, 4);
        await AssertPendingTeraMessages(agentDestination3, 3);
        await AssertPendingTeraMessages(agentDestination4, 2);
        await AssertPendingTeraMessages(agentDestination5, 1);

        await agentSource.Execute();
        await agentDestination1.Execute();
        await agentDestination2.Execute();
        await agentDestination3.Execute();
        await agentDestination4.Execute();
        await agentDestination5.Execute();

        AssertMessageCounts(agentSource, 0, 0);
        AssertMessageCounts(agentDestination1, 3, 2);
        AssertMessageCounts(agentDestination2, 3, 1);
        AssertMessageCounts(agentDestination3, 3, 0);
        AssertMessageCounts(agentDestination4, 2, 0);
        AssertMessageCounts(agentDestination5, 1, 0);

        await AssertCompletedTeraMessages(agentSource, 0);
        await AssertCompletedTeraMessages(agentDestination1, 5);
        await AssertCompletedTeraMessages(agentDestination2, 4);
        await AssertCompletedTeraMessages(agentDestination3, 3);
        await AssertCompletedTeraMessages(agentDestination4, 2);
        await AssertCompletedTeraMessages(agentDestination5, 1);

        await agentSource.Dispose();
        await agentDestination1.Dispose();
        await agentDestination2.Dispose();
        await agentDestination3.Dispose();
        await agentDestination4.Dispose();
        await agentDestination5.Dispose();
    }

    private static async Task TestBulkTeraMessagesBetweenAgents()
    {
        var agentSource = await GetTeraMessageTeraAgent();
        var agentDestination = await GetTeraMessageTeraAgent();
        var messages = GetRandomTeraMessages(agentSource, out var supportCount, out var notSupportCount).ToList();
        var relays = messages.Select(m => new TeraMessageRelayParameters
        {
            Code = m.Code,
            SourceTeraId = agentSource.TeraId,
            RecipientTeraIds = new[] {agentDestination.TeraId}
        });

        foreach (var relay in relays)
        {
            await agentSource.TeraMessageAgent.Send(relay);
        }

        await AssertPendingTeraMessages(agentSource, 0);
        await AssertPendingTeraMessages(agentDestination, messages.Count);

        await agentSource.Execute();
        await agentDestination.Execute();

        AssertMessageCounts(agentSource, 0, 0);
        AssertMessageCounts(agentDestination, supportCount, notSupportCount);

        await AssertCompletedTeraMessages(agentSource, 0);
        await AssertCompletedTeraMessages(agentDestination, messages.Count);

        await agentSource.Dispose();
        await agentDestination.Dispose();
    }

    private static async Task AssertPendingTeraMessages(ITeraAgent recipient, int numberOfMessages)
    {
        await using var scope = await recipient.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var messages = await dbContext.PendingTeraMessages
            .Include(p => p.TeraMessage)
            .Where(p => p.TeraMessage.RecipientTeraAgent.TeraId == recipient.TeraId)
            .ToListAsync();

        messages.Count.Should().Be(numberOfMessages);
        messages.All(m => m.TeraMessage.Status == TeraMessageStatus.Pending).Should().BeTrue();
    }

    private static async Task AssertCompletedTeraMessages(ITeraAgent recipient, int numberOfMessages)
    {
        await using var scope = await recipient.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var messages = await dbContext.TeraMessages
            .Where(m => m.RecipientTeraAgent.TeraId == recipient.TeraId)
            .ToListAsync();
        var pendingMessages = await dbContext.PendingTeraMessages
            .Where(p => p.TeraMessage.RecipientTeraAgent.TeraId == recipient.TeraId)
            .ToListAsync();

        pendingMessages.Should().BeEmpty();
        messages.Should().HaveCount(numberOfMessages);
        messages.All(m => m.Status == TeraMessageStatus.Completed).Should().BeTrue();
    }

    private static async Task SendTeraMessages(
        ITeraAgent source,
        int code,
        params ITeraAgent[] recipients)
    {
        var parameters = new TeraMessageRelayParameters
        {
            Code = code,
            SourceTeraId = source.TeraId,
            RecipientTeraIds = recipients.Select(t => t.TeraId)
        };

        await source.TeraMessageAgent.Send(parameters);
    }

    private static void AssertMessageCounts(ITeraAgent recipient, int supportedCount, int notSupportedCount)
    {
        var knowledge = (ITeraMessageKnowledgeAgent) recipient.Knowledge;

        knowledge.SupportedProcessCount.Should().Be(supportedCount);
        knowledge.NotSupportedProcessCount.Should().Be(notSupportedCount);
    }

    private static IEnumerable<TeraMessage> GetRandomTeraMessages(
        ITeraAgent source,
        out int numberOfSupportedMessages,
        out int numberOfNotSupportedMessages)
    {
        var result = new List<TeraMessage>();
        var random = new Random((int)DateTime.UtcNow.Ticks);

        for (var i = 0; i < TestBulkTeraMessages; i++)
        {
            var randomInt = random.Next(0, 2);
            var code = randomInt == 0 ? TeraMessageCodeConstants.SupportedTeraMessageCode1 : 1000;
            var message = new TeraMessage
            {
                SourceTeraId = source.TeraId,
                Code = code
            };

            result.Add(message);
        }

        numberOfSupportedMessages = result.Count(m => m.Code != 1000);
        numberOfNotSupportedMessages = result.Count(m => m.Code == 1000);

        return result;
    }

    private static async Task<ITeraAgent> GetTeraMessageTeraAgent()
    {
        var collapse = new NanoCollapseParameters
        {
            NanoTypeId = TeraMessageSplinterIds.TeraTypeId.Guid
        };

        var result = (await SplinterEnvironment.SuperpositionAgent.Collapse(collapse) as ITeraAgent)!;

        result.Should().NotBeNull();

        var init = new NanoInitialisationParameters();

        await result.Initialise(init);

        return result;
    }
}