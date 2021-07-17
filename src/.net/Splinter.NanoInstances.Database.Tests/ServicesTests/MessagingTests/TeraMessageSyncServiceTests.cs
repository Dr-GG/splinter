using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Messaging;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.MessagingTests
{
    [TestFixture]
    public class TeraMessageSyncServiceTests
    {
        private const long TestSenderTeraAgentId1 = 123;
        private const long TestRecipientTeraAgentId1 = 1232;

        public const long TestTeraMessageId01 = 432;
        public const long TestTeraMessageId02 = 32;
        public const long TestTeraMessageId03 = 43;
        public const long TestTeraMessageId04 = 4423;

        private const string TestErrorMessage1 = "Error message 1";
        private const string TestErrorMessage2 = "Error message 2";
        private const string TestErrorStackTrace1 = "Error stack trace 1";
        private const string TestErrorStackTrace2 = "Error stack trace 2";

        public static readonly DateTime TestCompletedTimestamp1 = new(2000, 01, 01, 15, 30, 0);
        public static readonly DateTime TestCompletedTimestamp2 = new(2000, 02, 01, 15, 30, 0);
        public static readonly DateTime TestCompletedTimestamp3 = new(2000, 01, 03, 15, 30, 0);
        public static readonly DateTime TestCompletedTimestamp4 = new(2000, 01, 01, 16, 30, 0);

        [Test]
        public async Task Sync_WhenGivingCompletedSyncsToPendingOrDequeuedMessages_MarksMessagesAsCompleted()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageSyncParameters
            {
                Syncs = new[]
                {
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId01,
                        CompletionTimestamp = TestCompletedTimestamp1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId02,
                        CompletionTimestamp = TestCompletedTimestamp2
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId03,
                        CompletionTimestamp = TestCompletedTimestamp3
                    }
                }
            };

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestTeraMessageId01, TeraMessageStatus.Pending);
            AddTeraMessage(dbContext, TestTeraMessageId02, TeraMessageStatus.Dequeued);
            AddTeraMessage(dbContext, TestTeraMessageId03, TeraMessageStatus.Dequeued);

            await service.Sync(parameters);

            AssertMessageCompleted(dbContext, TestTeraMessageId01, TestCompletedTimestamp1);
            AssertMessageCompleted(dbContext, TestTeraMessageId02, TestCompletedTimestamp2);
            AssertMessageCompleted(dbContext, TestTeraMessageId03, TestCompletedTimestamp3);
        }

        [Test]
        public async Task Sync_WhenGivingErrorCompletedSyncsToPendingOrDequeuedMessages_MarksMessagesAsFailed()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageSyncParameters
            {
                Syncs = new[]
                {
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId01,
                        CompletionTimestamp = TestCompletedTimestamp1,
                        ErrorCode = TeraMessageErrorCode.Disposed
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId02,
                        CompletionTimestamp = TestCompletedTimestamp2,
                        ErrorMessage = TestErrorMessage1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId03,
                        CompletionTimestamp = TestCompletedTimestamp3,
                        ErrorStackTrace = TestErrorStackTrace1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId04,
                        CompletionTimestamp = TestCompletedTimestamp4,
                        ErrorCode = TeraMessageErrorCode.MaximumDequeueCountReached,
                        ErrorMessage = TestErrorMessage2,
                        ErrorStackTrace = TestErrorStackTrace2
                    }
                }
            };

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestTeraMessageId01, TeraMessageStatus.Pending);
            AddTeraMessage(dbContext, TestTeraMessageId02, TeraMessageStatus.Dequeued);
            AddTeraMessage(dbContext, TestTeraMessageId03, TeraMessageStatus.Pending);
            AddTeraMessage(dbContext, TestTeraMessageId04, TeraMessageStatus.Dequeued);

            await service.Sync(parameters);

            AssertFailedMessage(dbContext, TestTeraMessageId01, TestCompletedTimestamp1, TeraMessageErrorCode.Disposed, null, null);
            AssertFailedMessage(dbContext, TestTeraMessageId02, TestCompletedTimestamp2, TeraMessageErrorCode.Unknown, TestErrorMessage1, null);
            AssertFailedMessage(dbContext, TestTeraMessageId03, TestCompletedTimestamp3, TeraMessageErrorCode.Unknown, null, TestErrorStackTrace1);
            AssertFailedMessage(dbContext, TestTeraMessageId04, TestCompletedTimestamp4, TeraMessageErrorCode.MaximumDequeueCountReached, TestErrorMessage2, TestErrorStackTrace2);
        }

        [Test]
        public async Task Sync_WhenGivingNonExistingMessageIds_OperatesNormally()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageSyncParameters
            {
                Syncs = new[]
                {
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId01,
                        CompletionTimestamp = TestCompletedTimestamp1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId02,
                        CompletionTimestamp = TestCompletedTimestamp2
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId03,
                        CompletionTimestamp = TestCompletedTimestamp3
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId04,
                        CompletionTimestamp = TestCompletedTimestamp4
                    }
                }
            };

            Assert.DoesNotThrowAsync(() => service.Sync(parameters));
        }

        [Test]
        public async Task Sync_WhenGivingCompletedSyncsToCancelledOrFailed_ResetsToCompleted()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageSyncParameters
            {
                Syncs = new[]
                {
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId01,
                        CompletionTimestamp = TestCompletedTimestamp1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId02,
                        CompletionTimestamp = TestCompletedTimestamp2
                    }
                }
            };

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestTeraMessageId01, TeraMessageStatus.Failed, TeraMessageErrorCode.Disposed, TestErrorMessage1, TestErrorStackTrace1);
            AddTeraMessage(dbContext, TestTeraMessageId02, TeraMessageStatus.Cancelled, TeraMessageErrorCode.Disposed, TestErrorMessage2, TestErrorStackTrace2);

            await service.Sync(parameters);

            AssertMessageCompleted(dbContext, TestTeraMessageId01, TestCompletedTimestamp1);
            AssertMessageCompleted(dbContext, TestTeraMessageId02, TestCompletedTimestamp2);
        }

        [Test]
        public async Task Sync_WhenGivingCompletedSyncsToCompletedMessages_IgnoresTheMessage()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new TeraMessageSyncParameters
            {
                Syncs = new[]
                {
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId01,
                        CompletionTimestamp = TestCompletedTimestamp1
                    },
                    new TeraMessageSyncParameter
                    {
                        TeraMessageId = TestTeraMessageId02,
                        CompletionTimestamp = TestCompletedTimestamp2
                    }
                }
            };

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestTeraMessageId01, TeraMessageStatus.Completed, completedTimestamp: TestCompletedTimestamp3);
            AddTeraMessage(dbContext, TestTeraMessageId02, TeraMessageStatus.Completed, completedTimestamp: TestCompletedTimestamp4);

            await service.Sync(parameters);

            AssertMessageCompleted(dbContext, TestTeraMessageId01, TestCompletedTimestamp3);
            AssertMessageCompleted(dbContext, TestTeraMessageId02, TestCompletedTimestamp4);
        }

        private static void AddTeraMessage(
            TeraDbContext dbContext,
            long id,
            TeraMessageStatus status,
            TeraMessageErrorCode? errorCode = null,
            string? errorMessage = null,
            string? errorStackTrace = null,
            DateTime? completedTimestamp = null)
        {
            var builder = new MockTeraDbContextBuilder(dbContext);

            builder.AddTeraMessage(
                TestSenderTeraAgentId1,
                TestRecipientTeraAgentId1,
                id,
                status: status,
                completedTimestamp: completedTimestamp,
                errorCode: errorCode,
                errorMessage: errorMessage,
                errorStackTrace: errorStackTrace,
                addPending: status == TeraMessageStatus.Dequeued
                            || status == TeraMessageStatus.Pending);
        }

        private static void AssertMessageCompleted(
            TeraDbContext dbContext, 
            long messageId,
            DateTime expectedCompletionTimestamp)
        {
            var message = dbContext.TeraMessages.Single(m => m.Id == messageId);
            var pending = dbContext.PendingTeraMessages.SingleOrDefault(p => p.TeraMessageId == messageId);

            Assert.IsNull(pending);

            Assert.AreEqual(TeraMessageStatus.Completed, message.Status);
            Assert.AreEqual(expectedCompletionTimestamp, message.CompletedTimestamp);
            Assert.IsNull(message.ErrorCode);
            Assert.IsNull(message.ErrorStackTrace);
            Assert.IsNull(message.ErrorMessage);
        }

        private static void AssertFailedMessage(
            TeraDbContext dbContext,
            long messageId,
            DateTime expectedCompletionTimestamp,
            TeraMessageErrorCode errorCode,
            string? errorMessage,
            string? errorStacktrace)
        {
            var message = dbContext.TeraMessages.Single(m => m.Id == messageId);
            var pending = dbContext.PendingTeraMessages.SingleOrDefault(p => p.TeraMessageId == messageId);

            Assert.IsNull(pending);

            Assert.AreEqual(TeraMessageStatus.Failed, message.Status);
            Assert.AreEqual(expectedCompletionTimestamp, message.CompletedTimestamp);
            Assert.AreEqual(errorCode, message.ErrorCode);
            Assert.AreEqual(errorMessage, message.ErrorMessage);
            Assert.AreEqual(errorStacktrace, message.ErrorStackTrace);
        }

        private static void AddDefaultData(TeraDbContext dbContext)
        {
            var builder = new MockTeraDbContextBuilder(dbContext);

            builder.AddTeraAgent(TestRecipientTeraAgentId1);
            builder.AddTeraAgent(TestSenderTeraAgentId1);
        }

        private static ITeraMessageSyncService GetService(TeraDbContext dbContext)
        {
            return new TeraMessageSyncService(
                new TeraMessagingSettings
                {
                    MaximumSyncRetryCount = 3
                },
                dbContext);
        }
    }
}
