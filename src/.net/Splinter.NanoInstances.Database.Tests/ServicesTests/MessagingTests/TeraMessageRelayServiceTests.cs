using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Messaging;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.MessagingTests;

[TestFixture]
public class TeraMessageRelayServiceTests
{
    private static readonly TimeSpan DefaultAbsoluteExpiryTimeSpan = TimeSpan.FromHours(4);

    private const int TestPriority = 124;
    private const string TestMessage = "Some message";

    private static readonly Guid TestTeraAgentId1 = new ("{FE279069-CBF5-44EA-8DAA-C77351781FEE}");
    private static readonly Guid TestTeraAgentId2 = new ("{FE279069-CBF5-44EA-8DAA-C77351781FEE}");
    private static readonly Guid TestTeraAgentId3 = new ("{9CBDE3B4-18DE-4DFA-902E-A89BA49340B9}");
    private static readonly Guid TestTeraAgentId4 = new ("{E98583AD-1FA3-4932-81D1-34F6866465B8}");
    private static readonly Guid TestTeraAgentId5 = new ("{2352C821-A63E-4CFC-B735-E50DAF668210}");
    private static readonly Guid TestTeraAgentId6 = new ("{2B79A66E-2416-4E46-B263-63155C80F0F3}");
    private static readonly Guid TestTeraAgentId7 = new ("{A646738A-86A4-4791-B874-B467F72B3D66}");

    private static readonly Guid TestTeraAgentNonExistingId1 = new("{BCF3FEEC-1E16-42B9-B485-472BABEC0826}");
    private static readonly Guid TestTeraAgentNonExistingId2 = new("{35D5762F-0AA1-405A-A9F5-32B05E68E6E4}");
    private static readonly Guid TestTeraAgentNonExistingId3 = new("{C7E3615D-034C-4926-8C6F-72BBF70BA519}");

    private static readonly Guid SenderTeraAgentId = TestTeraAgentId7;

    private static readonly IEnumerable<Guid> TestNotFoundTeraAgentIds = new[]
    {
        TestTeraAgentNonExistingId1,
        TestTeraAgentNonExistingId2,
        TestTeraAgentNonExistingId3
    };

    private static readonly IEnumerable<Guid> TestRecipientTeraAgentIds = new[]
    {
        TestTeraAgentId1,
        TestTeraAgentId2,
        TestTeraAgentId3,
        TestTeraAgentId4,
        TestTeraAgentId5,
        TestTeraAgentId6,
        TestTeraAgentNonExistingId1,
        TestTeraAgentNonExistingId2,
        TestTeraAgentNonExistingId3
    };

    private static readonly IEnumerable<Guid> TestRunningTeraAgentIds = new[]
    {
        TestTeraAgentId3,
        TestTeraAgentId5
    };

    private static readonly IEnumerable<Guid> TestMigratingTeraAgentIds = new[]
    {
        TestTeraAgentId2
    };

    private static readonly IEnumerable<Guid> TestDisposedTeraAgentIds = new[]
    {
        TestTeraAgentId1,
        TestTeraAgentId4,
        TestTeraAgentId6
    };

    [Test]
    public async Task Relay_WhenGivenATeraMessage_ReturnsTheExpectedResponse()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var parameters = new TeraMessageRelayParameters
        {
            SourceTeraId = SenderTeraAgentId,
            Message = TestMessage,
            Priority = TestPriority,
            RecipientTeraIds = TestRecipientTeraAgentIds
        };

        AddDefaultData(dbContext);

        var response = await service.Relay(parameters);

        Assert.AreEqual(
            TestDisposedTeraAgentIds.Count(),
            response.TeraIdsDisposed.Count());

        Assert.AreEqual(
            TestNotFoundTeraAgentIds.Count(),
            response.TeraIdsNotFounds.Count());

        Assert.IsTrue(response.TeraIdsDisposed
            .All(id => TestDisposedTeraAgentIds.Contains(id)));

        Assert.IsTrue(response.TeraIdsNotFounds
            .All(id => TestNotFoundTeraAgentIds.Contains(id)));

        AssertTeraMessages(response.TeraAgentMessageIds, dbContext);
        Assert.AreEqual(TestMigratingTeraAgentIds.Count()
                        + TestRunningTeraAgentIds.Count(), dbContext.TeraMessages.Count());
        Assert.AreEqual(TestMigratingTeraAgentIds.Count()
                        + TestRunningTeraAgentIds.Count(), dbContext.PendingTeraMessages.Count());
    }

    [Test]
    public async Task Relay_WhenProvidingExplicitExpiryTimespans_SetsAppropriateExpiryTimestamps()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var absoluteTimespan = TimeSpan.FromMinutes(15);
        var parameters = new TeraMessageRelayParameters
        {
            SourceTeraId = SenderTeraAgentId,
            Message = TestMessage,
            Priority = TestPriority,
            RecipientTeraIds = TestRecipientTeraAgentIds,
            AbsoluteExpiryTimeSpan = absoluteTimespan
        };

        AddDefaultData(dbContext);

        var response = await service.Relay(parameters);

        foreach (var message in response.TeraAgentMessageIds)
        {
            AssertTeraMessageAbsoluteExpiry(dbContext, message.MessageId, absoluteTimespan);
        }
    }

    [Test]
    public async Task Relay_WhenProvidingNoExpiryTimespans_SetsDefaultExpiryTimestamps()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var service = GetService(dbContext);
        var parameters = new TeraMessageRelayParameters
        {
            SourceTeraId = SenderTeraAgentId,
            Message = TestMessage,
            Priority = TestPriority,
            RecipientTeraIds = TestRecipientTeraAgentIds
        };

        AddDefaultData(dbContext);

        var response = await service.Relay(parameters);

        foreach (var message in response.TeraAgentMessageIds)
        {
            AssertTeraMessageAbsoluteExpiry(dbContext, message.MessageId, DefaultAbsoluteExpiryTimeSpan);
        }
    }

    private static void AssertTeraMessageAbsoluteExpiry(
        TeraDbContext dbContext,
        long id,
        TimeSpan expiryTimespan)
    {
        var message = dbContext.TeraMessages.Single(m => m.Id == id);

        AssertExpiryTimestamp(message.AbsoluteExpiryTimestamp, expiryTimespan);
    }

    private static void AssertExpiryTimestamp(DateTime expiryTimestamp, TimeSpan expiryTimeSpan)
    {
        var now = DateTime.UtcNow;
        var from = now.AddMinutes(-1);
        var to = now.Add(expiryTimeSpan).AddMinutes(1);

        Assert.IsTrue(expiryTimestamp >= from && expiryTimestamp <= to);
    }

    private static void AssertTeraMessages(
        IEnumerable<TeraAgentMessageResponse> responses,
        TeraDbContext teraDbContext)
    {
        foreach (var response in responses)
        {
            AssertTeraMessage(response, teraDbContext);
        }
    }

    private static void AssertTeraMessage(
        TeraAgentMessageResponse response,
        TeraDbContext teraDbContext)
    {
        var teraMessage = teraDbContext.TeraMessages
            .Single(m => m.Id == response.MessageId
                         && m.RecipientTeraAgent.TeraId == response.TeraId);
        var pendingMessage = teraDbContext.PendingTeraMessages
            .Single(p => p.TeraMessageId == response.MessageId
                         && p.TeraAgent.TeraId == response.TeraId);

        Assert.AreEqual(TestMessage, teraMessage.Message);
        Assert.AreEqual(TestPriority, teraMessage.Priority);
        Assert.AreEqual(SenderTeraAgentId, teraMessage.SourceTeraAgent.TeraId);
        Assert.AreEqual(TeraMessageStatus.Pending, teraMessage.Status);

        Assert.IsNull(teraMessage.CompletedTimestamp);
        Assert.IsNull(teraMessage.DequeuedTimestamp);
        Assert.IsNull(teraMessage.ErrorCode);
        Assert.IsNull(teraMessage.ErrorMessage);
        Assert.IsNull(teraMessage.ErrorStackTrace);

        Assert.AreEqual(teraMessage.RecipientTeraAgentId, pendingMessage.TeraAgentId);
    }

    private static void AddDefaultData(TeraDbContext teraDbContext)
    {
        AddTeraAgents(teraDbContext);
    }

    private static void AddTeraAgents(TeraDbContext teraDbContext)
    {
        AddTeraAgent(teraDbContext, SenderTeraAgentId, TeraAgentStatus.Running);

        foreach (var id in TestRunningTeraAgentIds)
        {
            AddTeraAgent(teraDbContext, id, TeraAgentStatus.Running);
        }

        foreach (var id in TestMigratingTeraAgentIds)
        {
            AddTeraAgent(teraDbContext, id, TeraAgentStatus.Migrating);
        }

        foreach (var id in TestDisposedTeraAgentIds)
        {
            AddTeraAgent(teraDbContext, id, TeraAgentStatus.Disposed);
        }
    }

    private static void AddTeraAgent(
        TeraDbContext teraDbContext,
        Guid teraId, 
        TeraAgentStatus status)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);
        var agent = builder.AddTeraAgent(teraId);

        agent.Status = status;

        builder.Save();
    }

    private static ITeraMessageRelayService GetService(TeraDbContext teraDbContext)
    {
        return new TeraMessageRelayService(
            new TeraMessagingSettings
            {
                Disposing = new TeraMessageDisposeSettings
                {
                    DefaultExpiryTimeSpan = DefaultAbsoluteExpiryTimeSpan
                }
            },
            teraDbContext);
    }
}