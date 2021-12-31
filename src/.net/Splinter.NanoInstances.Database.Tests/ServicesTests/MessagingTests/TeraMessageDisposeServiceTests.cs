using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Messaging;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.MessagingTests
{
    [TestFixture]
    public class TeraMessageDisposeServiceTests
    {
        private const long TestRecipientTeraAgentId1 = 23;
        private const long TestSenderTeraAgentId = 321;
        private const long TestMessageId01 = 23;
        private const long TestMessageId02 = 45;
        private const long TestMessageId03 = 56;
        private const long TestMessageId04 = 67;
        private const long TestMessageId05 = 78;
        private const long TestMessageId06 = 98;
        private const long TestMessageId07 = 433;
        private const long TestMessageId08 = 223;
        private const long TestMessageId09 = 667;
        private const long TestMessageId10 = 889;
        private const long TestMessageId11 = 998;
        private const long TestMessageId12 = 2314;

        private const int ExpiryOneMinute = 1000 * 60;
        private const int ExpiryTwoMinutes = ExpiryOneMinute * 2;
        private const int ExpiryThreeMinutes = ExpiryOneMinute * 3;

        private const int TestMaximumNumberOfMessagesToDispose = 5;

        [Test]
        public async Task Dispose_WhenMessagesAreReadyToBeDisposedOf_DisposesTheCorrectMessages()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var disposer = GetTeraMessageDisposer(dbContext);

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestMessageId01, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId02, TeraMessageStatus.Dequeued, 100);
            AddTeraMessage(dbContext, TestMessageId03, TeraMessageStatus.Dequeued, 50);
            AddTeraMessage(dbContext, TestMessageId04, TeraMessageStatus.Pending, 25);

            AddTeraMessage(dbContext, TestMessageId05, TeraMessageStatus.Cancelled, 500);
            AddTeraMessage(dbContext, TestMessageId06, TeraMessageStatus.Completed, 400);
            AddTeraMessage(dbContext, TestMessageId07, TeraMessageStatus.Failed, 200);

            AddTeraMessage(dbContext, TestMessageId08, TeraMessageStatus.Pending, ExpiryOneMinute);
            AddTeraMessage(dbContext, TestMessageId09, TeraMessageStatus.Dequeued, ExpiryTwoMinutes);

            AddTeraMessage(dbContext, TestMessageId10, TeraMessageStatus.Cancelled, ExpiryOneMinute);
            AddTeraMessage(dbContext, TestMessageId11, TeraMessageStatus.Completed, ExpiryTwoMinutes);
            AddTeraMessage(dbContext, TestMessageId12, TeraMessageStatus.Failed, ExpiryThreeMinutes);

            Thread.Sleep(1000);

            disposer.DisposeMessages();

            AssertMessageIsDisposed(dbContext, TestMessageId01);
            AssertMessageIsDisposed(dbContext, TestMessageId02);
            AssertMessageIsDisposed(dbContext, TestMessageId03);
            AssertMessageIsDisposed(dbContext, TestMessageId04);

            AssertMessage(dbContext, TestMessageId05, TeraMessageStatus.Cancelled, false);
            AssertMessage(dbContext, TestMessageId06, TeraMessageStatus.Completed, false);
            AssertMessage(dbContext, TestMessageId07, TeraMessageStatus.Failed, false);
            AssertMessage(dbContext, TestMessageId08, TeraMessageStatus.Pending, true);
            AssertMessage(dbContext, TestMessageId09, TeraMessageStatus.Dequeued, true);
            AssertMessage(dbContext, TestMessageId10, TeraMessageStatus.Cancelled, false);
            AssertMessage(dbContext, TestMessageId11, TeraMessageStatus.Completed, false);
            AssertMessage(dbContext, TestMessageId12, TeraMessageStatus.Failed, false);
        }

        [Test]
        public async Task Dispose_WhenMessagesAreReadyToBeDisposedOf_DisposesTheMaximumNumberOfMessages()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var disposer = GetTeraMessageDisposer(dbContext);

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestMessageId01, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId02, TeraMessageStatus.Dequeued, 100);
            AddTeraMessage(dbContext, TestMessageId03, TeraMessageStatus.Dequeued, 50);
            AddTeraMessage(dbContext, TestMessageId04, TeraMessageStatus.Pending, 25);
            AddTeraMessage(dbContext, TestMessageId05, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId06, TeraMessageStatus.Dequeued, 100);
            AddTeraMessage(dbContext, TestMessageId07, TeraMessageStatus.Dequeued, 50);
            AddTeraMessage(dbContext, TestMessageId08, TeraMessageStatus.Pending, 25);
            AddTeraMessage(dbContext, TestMessageId09, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId10, TeraMessageStatus.Dequeued, 100);

            Thread.Sleep(1000);

            disposer.DisposeMessages();

            AssertMessageIsDisposed(dbContext, TestMessageId01);
            AssertMessageIsDisposed(dbContext, TestMessageId02);
            AssertMessageIsDisposed(dbContext, TestMessageId03);
            AssertMessageIsDisposed(dbContext, TestMessageId04);
            AssertMessageIsDisposed(dbContext, TestMessageId05);

            AssertMessage(dbContext, TestMessageId06, TeraMessageStatus.Dequeued, true);
            AssertMessage(dbContext, TestMessageId07, TeraMessageStatus.Dequeued, true);
            AssertMessage(dbContext, TestMessageId08, TeraMessageStatus.Pending, true);
            AssertMessage(dbContext, TestMessageId09, TeraMessageStatus.Pending, true);
            AssertMessage(dbContext, TestMessageId10, TeraMessageStatus.Dequeued, true);
        }

        [Test]
        public async Task Dispose_WhenMessagesAreReadyToBeDisposedOfMultipleTimes_DisposesAllMessages()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var disposer = GetTeraMessageDisposer(dbContext);

            AddDefaultData(dbContext);

            AddTeraMessage(dbContext, TestMessageId01, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId02, TeraMessageStatus.Dequeued, 100);
            AddTeraMessage(dbContext, TestMessageId03, TeraMessageStatus.Dequeued, 50);
            AddTeraMessage(dbContext, TestMessageId04, TeraMessageStatus.Pending, 25);
            AddTeraMessage(dbContext, TestMessageId05, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId06, TeraMessageStatus.Dequeued, 100);
            AddTeraMessage(dbContext, TestMessageId07, TeraMessageStatus.Dequeued, 50);
            AddTeraMessage(dbContext, TestMessageId08, TeraMessageStatus.Pending, 25);
            AddTeraMessage(dbContext, TestMessageId09, TeraMessageStatus.Pending, 200);
            AddTeraMessage(dbContext, TestMessageId10, TeraMessageStatus.Dequeued, 100);

            Thread.Sleep(1000);

            disposer.DisposeMessages();
            disposer.DisposeMessages();

            AssertMessageIsDisposed(dbContext, TestMessageId01);
            AssertMessageIsDisposed(dbContext, TestMessageId02);
            AssertMessageIsDisposed(dbContext, TestMessageId03);
            AssertMessageIsDisposed(dbContext, TestMessageId04);
            AssertMessageIsDisposed(dbContext, TestMessageId05);
            AssertMessageIsDisposed(dbContext, TestMessageId06);
            AssertMessageIsDisposed(dbContext, TestMessageId07);
            AssertMessageIsDisposed(dbContext, TestMessageId08);
            AssertMessageIsDisposed(dbContext, TestMessageId09);
            AssertMessageIsDisposed(dbContext, TestMessageId10);
        }

        private static void AddDefaultData(TeraDbContext dbContext)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraAgent(TestSenderTeraAgentId);
            builder.AddTeraAgent(TestRecipientTeraAgentId1);
        }

        private static void AddTeraMessage(
            TeraDbContext dbContext,
            long id,
            TeraMessageStatus status,
            int millisecondsExpiryOffset)
        {
            var builder = new MockTeraDataBuilder(dbContext);

            builder.AddTeraMessage(
                TestSenderTeraAgentId,
                TestRecipientTeraAgentId1,
                id,
                status: status,
                absoluteExpiryTimestamp: DateTime.UtcNow.AddMilliseconds(millisecondsExpiryOffset),
                addPending: status == TeraMessageStatus.Dequeued
                            || status == TeraMessageStatus.Pending);
        }

        private static void AssertMessageIsDisposed(TeraDbContext dbContext, long id)
        {
            var message = dbContext.TeraMessages.Single(m => m.Id == id);
            var pending = dbContext.PendingTeraMessages.SingleOrDefault(p => p.TeraMessageId == id);
            
            Assert.AreEqual(TeraMessageStatus.Cancelled, message.Status);
            Assert.AreEqual(TeraMessageErrorCode.Disposed, message.ErrorCode);
            Assert.IsNull(message.ErrorMessage);
            Assert.IsNull(message.ErrorStackTrace);
            Assert.IsTrue(message.CompletedTimestamp <= DateTime.UtcNow);

            Assert.IsNull(pending);
        }

        private static void AssertMessage(
            TeraDbContext dbContext, 
            long id, 
            TeraMessageStatus status, 
            bool hasPending)
        {
            var message = dbContext.TeraMessages.Single(m => m.Id == id);
            var pending = dbContext.PendingTeraMessages.SingleOrDefault(p => p.TeraMessageId == id);

            Assert.IsTrue(hasPending ? pending != null : pending == null);
            Assert.AreEqual(status, message.Status);
            Assert.IsNull(message.ErrorCode);
            Assert.IsNull(message.ErrorMessage);
            Assert.IsNull(message.ErrorStackTrace);
        }

        private static ITeraMessageDisposeService GetTeraMessageDisposer(TeraDbContext dbContext)
        {
            return new TeraMessageDisposeService(new TeraMessagingSettings
            {
                Disposing = new TeraMessageDisposeSettings
                {
                    MaximumNumberOfMessagesToDispose = TestMaximumNumberOfMessagesToDispose
                }
            }, dbContext);
        }
    }
}
