using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.Messaging;
using Splinter.NanoInstances.Default.Tests.Agents.NanoAgents;
using Splinter.NanoInstances.Environment;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Domain.Messaging;
using Splinter.NanoTypes.Domain.Parameters.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.MessagingTests
{
    [TestFixture]
    public class TeraMessageQueueTests
    {
        private const int TestMaximumNumberOfMessages = 12;

        [Test]
        public void HasNext_WhenNotInitialised_ThrowsException()
        {
            var queue = GetDefaultQueue();

            Assert.ThrowsAsync<NanoServiceNotInitialisedException>(() => queue.HasNext());
        }

        [Test]
        public void Next_WhenNoMessageIsAvailable_ThrowsException()
        {
            var queue = GetDefaultQueue();

            DefaultInitialise(queue);

            Assert.ThrowsAsync<InvalidNanoServiceOperationException>(() => queue.Next());
        }

        [Test]
        public async Task HasNextAndNext_WhenMessagesAreAvailable_ReturnsMessagesAsExpected()
        {
            var queue = GetDefaultQueue();
            var dequeuedMessages = 0;
            var index = 0;
            var mockMessageAgent = GetDefaultTeraMessageAgent();

            SplinterEnvironment.TeraMessageAgent = mockMessageAgent.Object;

            DefaultInitialise(queue);

            while (await queue.HasNext())
            {
                var message = await queue.Next();

                dequeuedMessages++;

                AssertTeraMessage(message, ++index);
            }

            Assert.AreEqual(TestMaximumNumberOfMessages, dequeuedMessages);

            mockMessageAgent.Verify(
                m => m.Dequeue(It.IsAny<TeraMessageDequeueParameters>()),
                Times.Exactly(TestMaximumNumberOfMessages / 2 + 1));
        }

        private static TeraMessage ConstructNextTeraMessage(int index)
        {
            return new TeraMessage
            {
                Id = index,
                Message = $"Message {index}"
            };
        }

        private static void AssertTeraMessage(TeraMessage message, int index)
        {
            Assert.AreEqual(index, message.Id);
            Assert.AreEqual($"Message {index}", message.Message);
        }

        private static Mock<ITeraMessageAgent> GetDefaultTeraMessageAgent()
        {
            var mock = new Mock<ITeraMessageAgent>();
            var index = 0;

            mock
                .Setup(m => m.Dequeue(It.IsAny<TeraMessageDequeueParameters>()))
                .ReturnsAsync(() =>
                {
                    return index >= TestMaximumNumberOfMessages
                        ? Enumerable.Empty<TeraMessage>()
                        : new[]
                        {
                            ConstructNextTeraMessage(++index),
                            ConstructNextTeraMessage(++index)
                        };
                });

            return mock;
        }

        private static void DefaultInitialise(ITeraMessageQueue queue)
        {
            var teraAgent = new UnitTestTeraAgent();

            queue.Initialise(teraAgent);
        }

        private static ITeraMessageQueue GetDefaultQueue()
        {
            return new TeraMessageQueue(new TeraMessagingSettings());
        }
    }
}
