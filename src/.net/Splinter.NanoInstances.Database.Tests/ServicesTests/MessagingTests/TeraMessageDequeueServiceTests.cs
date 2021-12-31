using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Messaging;
using Splinter.NanoInstances.Database.Tests.Utilities;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.MessagingTests
{
    [TestFixture]
    public class TeraMessageDequeueServiceTests
    {
        private const int TestMaximumDequeueCount = 10;

        private const int TestTeraMessageId01 = 1223;
        private const int TestTeraMessageId02 = 3;
        private const int TestTeraMessageId03 = 23;
        private const int TestTeraMessageId04 = 123;
        private const int TestTeraMessageId05 = 176;
        private const int TestTeraMessageId06 = 7663;
        private const int TestTeraMessageId07 = 1;
        private const int TestTeraMessageId08 = 90;
        private const int TestTeraMessageId09 = 232;
        private const int TestTeraMessageId10 = 676;
        private const int TestTeraMessageId11 = 6176;
        private const int TestTeraMessageId12 = 6736;

        private const long TestPlatformId = 21;
        private const long TestNanoInstanceId = 555;

        private const long TestRecipientTeraAgentId1 = 2559;
        private const long TestRecipientTeraAgentId2 = 9875;
        private const long TestSenderTeraAgentId1 = 62525;
        private const long TestSenderTeraAgentId2 = 625;

        private static readonly Guid TestRecipientTeraId1 = new("{EE69B229-C24A-4DA0-8E8C-58FA9FE5D53C}");
        private static readonly Guid TestRecipientTeraId2 = new("{EE125437-70DF-4042-9BFF-48B067ABE0E8}");
        private static readonly Guid TestSenderTeraId1 = new("{EF0DF904-F0AC-4257-A011-7CE6620618E8}");
        private static readonly Guid TestSenderTeraId2 = new("{AB486A5D-1107-4B37-94E6-5172B890C79E}");

        [Test]
        public async Task Dequeue_WhenTeraMessagesArePending_ReturnsOnlyPendingMessagesAndMarksAsDequeued()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageDequeueParameters
            {
                MaximumNumberOfTeraMessages = 10,
                TeraId = TestRecipientTeraId1
            };

            AddDefaultData(dbContext);

            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId01);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId2, TestRecipientTeraAgentId1, TestTeraMessageId02);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId03);

            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId2, TestTeraMessageId04);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId2, TestRecipientTeraAgentId2, TestTeraMessageId05);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId2, TestTeraMessageId06);

            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId07);
            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId2, TestRecipientTeraAgentId1, TestTeraMessageId08);
            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId09);

            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId2, TestTeraMessageId10);
            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId2, TestRecipientTeraAgentId2, TestTeraMessageId11);
            AddDequeuedTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId2, TestTeraMessageId12);

            var messages = (await service.Dequeue(parameters)).ToList();

            Assert.AreEqual(3, messages.Count);

            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId01, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId02, TestSenderTeraAgentId2, TestSenderTeraId2, 1);
            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId03, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
        }

        [Test]
        public async Task Dequeue_WhenTeraMessageHasReachedMaximumDequeueCount_IsCancelledAndNotReturned()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageDequeueParameters
            {
                MaximumNumberOfTeraMessages = 10,
                TeraId = TestRecipientTeraId1
            };

            AddDefaultData(dbContext);

            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId01);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId2, TestRecipientTeraAgentId1, TestTeraMessageId02);
            AddPendingTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId03);

            AddToBeCancelledTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId04);
            AddToBeCancelledTeraMessage(dbContext, TestSenderTeraAgentId1, TestRecipientTeraAgentId1, TestTeraMessageId05);

            var messages = (await service.Dequeue(parameters)).ToList();

            Assert.AreEqual(3, messages.Count);

            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId01, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId02, TestSenderTeraAgentId2, TestSenderTeraId2, 1);
            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId03, TestSenderTeraAgentId1, TestSenderTeraId1, 1);

            AssertCancelledMessage(dbContext, TestTeraMessageId04);
            AssertCancelledMessage(dbContext, TestTeraMessageId05);
        }

        [Test]
        public async Task Dequeue_WhenGivenPriorities_SortsMessagesAccordingly()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageDequeueParameters
            {
                MaximumNumberOfTeraMessages = 10,
                TeraId = TestRecipientTeraId1
            };

            AddDefaultData(dbContext);

            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId01, 1, new DateTime(2001, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId02, 1, new DateTime(2001, 01, 02));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId03, 1, new DateTime(2001, 01, 03));

            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId04, 2, new DateTime(2000, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId05, 3, new DateTime(2000, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId06, 4, new DateTime(2000, 01, 01));

            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId07, 5, new DateTime(2000, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId08, 5, new DateTime(2000, 01, 02));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId09, 5, new DateTime(2000, 01, 03));

            var messages = (await service.Dequeue(parameters)).ToList();

            Assert.AreEqual(9, messages.Count);
            Assert.AreEqual(TestTeraMessageId07, messages[0].Id);
            Assert.AreEqual(TestTeraMessageId08, messages[1].Id);
            Assert.AreEqual(TestTeraMessageId09, messages[2].Id);
            Assert.AreEqual(TestTeraMessageId06, messages[3].Id);
            Assert.AreEqual(TestTeraMessageId05, messages[4].Id);
            Assert.AreEqual(TestTeraMessageId04, messages[5].Id);
            Assert.AreEqual(TestTeraMessageId01, messages[6].Id);
            Assert.AreEqual(TestTeraMessageId02, messages[7].Id);
            Assert.AreEqual(TestTeraMessageId03, messages[8].Id);
        }

        [Test]
        public async Task Dequeue_WhenMaximumNumberIsSmallerThanAvailableMessages_ReturnsOnlyTheCorrectNumberOfMessages()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageDequeueParameters
            {
                MaximumNumberOfTeraMessages = 1,
                TeraId = TestRecipientTeraId1
            };

            AddDefaultData(dbContext);

            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId01, 1, new DateTime(2001, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId02, 1, new DateTime(2001, 01, 02));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId03, 1, new DateTime(2001, 01, 03));

            var messages = (await service.Dequeue(parameters)).ToList();

            Assert.AreEqual(1, messages.Count);

            AssertDequeuedMessage(dbContext, messages, TestTeraMessageId01, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
        }

        [Test]
        public async Task Dequeue_WhenDequeuingMessagesLinearly_ReturnsTheNextMessage()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageDequeueParameters
            {
                MaximumNumberOfTeraMessages = 1,
                TeraId = TestRecipientTeraId1
            };

            AddDefaultData(dbContext);

            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId01, 1, new DateTime(2001, 01, 01));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId02, 1, new DateTime(2001, 01, 02));
            AddPrioritisedTeraMessage(dbContext, TestTeraMessageId03, 1, new DateTime(2001, 01, 03));

            var messages1 = (await service.Dequeue(parameters)).ToList();
            var messages2 = (await service.Dequeue(parameters)).ToList();
            var messages3 = (await service.Dequeue(parameters)).ToList();

            Assert.AreEqual(1, messages1.Count);
            Assert.AreEqual(1, messages2.Count);
            Assert.AreEqual(1, messages3.Count);

            AssertDequeuedMessage(dbContext, messages1, TestTeraMessageId01, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
            AssertDequeuedMessage(dbContext, messages2, TestTeraMessageId02, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
            AssertDequeuedMessage(dbContext, messages3, TestTeraMessageId03, TestSenderTeraAgentId1, TestSenderTeraId1, 1);
        }

        private static void AssertCancelledMessage(
            TeraDbContext dbContext,
            long messageId)
        {
            var dbTeraMessage = dbContext.TeraMessages.Single(m => m.Id == messageId);
            var pendingMessage = dbContext.PendingTeraMessages.SingleOrDefault(p => p.TeraMessageId == messageId);

            Assert.IsNull(pendingMessage);

            Assert.AreEqual(TeraMessageStatus.Cancelled, dbTeraMessage.Status);
            Assert.AreEqual(TeraMessageErrorCode.MaximumDequeueCountReached, dbTeraMessage.ErrorCode);
            Assert.AreEqual(TeraMessageErrorCode.MaximumDequeueCountReached, dbTeraMessage.ErrorCode);

            Assert.IsTrue(dbTeraMessage.CompletedTimestamp <= DateTime.UtcNow);
        }

        private static void AssertDequeuedMessage(
            TeraDbContext dbContext,
            IEnumerable<TeraMessage> teraMessages,
            long messageId,
            long expectedSenderTeraAgentId,
            Guid expectedSenderTeraId,
            int expectedDequeueCount)
        {
            var responseTeraMessage = teraMessages.Single(m => m.Id == messageId);
            var dbTeraMessage = dbContext.TeraMessages.Single(m => m.Id == messageId);

            Assert.AreEqual(TeraMessageStatus.Dequeued, responseTeraMessage.Status);
            Assert.AreEqual(TeraMessageStatus.Dequeued, dbTeraMessage.Status);
            Assert.AreEqual(expectedDequeueCount, responseTeraMessage.DequeueCount);
            Assert.AreEqual(expectedDequeueCount, dbTeraMessage.DequeueCount);
            Assert.AreEqual(expectedSenderTeraId, responseTeraMessage.SourceTeraId);
            Assert.AreEqual(expectedSenderTeraAgentId, dbTeraMessage.SourceTeraAgentId);

            Assert.IsTrue(responseTeraMessage.DequeuedTimestamp <= DateTime.UtcNow);
            Assert.IsTrue(dbTeraMessage.DequeuedTimestamp <= DateTime.UtcNow);
        }

        private static void AddDefaultData(TeraDbContext dbContext)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraPlatform(TestPlatformId);
            builder.AddNanoInstance(TestNanoInstanceId);

            AddTeraAgent(dbContext, TestRecipientTeraAgentId1, TestRecipientTeraId1);
            AddTeraAgent(dbContext, TestRecipientTeraAgentId2, TestRecipientTeraId2);
            AddTeraAgent(dbContext, TestSenderTeraAgentId1, TestSenderTeraId1);
            AddTeraAgent(dbContext, TestSenderTeraAgentId2, TestSenderTeraId2);
        }

        private static void AddTeraAgent(TeraDbContext dbContext, long teraAgentId, Guid teraId)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraAgent(
                TestPlatformId,
                TestNanoInstanceId,
                teraAgentId,
                teraId);
        }

        private static void AddDequeuedTeraMessage(
            TeraDbContext dbContext,
            long sourceTeraId,
            long recipientTeraId,
            long id,
            int priority = 0)
        {
            AddTeraMessage(
                dbContext,
                id,
                sourceTeraId,
                recipientTeraId,
                priority,
                0,
                TeraMessageStatus.Dequeued);
        }

        private static void AddPendingTeraMessage(
            TeraDbContext dbContext,
            long sourceTeraId,
            long recipientTeraId,
            long id,
            int priority = 0)
        {
            AddTeraMessage(
                dbContext, 
                id,
                sourceTeraId,
                recipientTeraId,
                priority,
                0,
                TeraMessageStatus.Pending);
        }

        private static void AddPrioritisedTeraMessage(
            TeraDbContext dbContext,
            long id,
            int priority,
            DateTime loggedTimestamp)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraMessage(
                TestSenderTeraAgentId1,
                TestRecipientTeraAgentId1,
                id,
                priority,
                status: TeraMessageStatus.Pending,
                loggedTimestamp: loggedTimestamp);
        }

        private static void AddToBeCancelledTeraMessage(
            TeraDbContext dbContext,
            long sourceTeraId,
            long recipientTeraId,
            long id)
        {
            AddTeraMessage(
                dbContext,
                id,
                sourceTeraId,
                recipientTeraId,
                0,
                TestMaximumDequeueCount,
                TeraMessageStatus.Pending);
        }

        private static void AddTeraMessage(
            TeraDbContext dbContext,
            long id,
            long sourceTeraId,
            long recipientTeraId,
            int priority,
            int dequeueCount,
            TeraMessageStatus status)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraMessage(
                sourceTeraId,
                recipientTeraId,
                id,
                priority,
                status: status,
                dequeueCount: dequeueCount);
        }

        private static ITeraMessageDequeueService GetService(TeraDbContext dbContext)
        {
            return new TeraMessageDequeueService(
                new TeraMessagingSettings
                {
                    MaximumDequeueRetryCount = TestMaximumDequeueCount
                },
                dbContext,
                MockDatabaseMapperUtilities.GetTeraMessageMapper());
        }
    }
}
