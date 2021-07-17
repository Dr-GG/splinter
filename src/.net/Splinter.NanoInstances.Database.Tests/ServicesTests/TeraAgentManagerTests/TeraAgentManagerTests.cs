using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.TeraAgents;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.TeraAgentManagerTests
{
    [TestFixture]
    public class TeraAgentManagerTests
    {
        private const long TestTeraAgentId = 541;

        private static readonly Guid TestTeraId = new("{46555385-7B5E-4CFA-B1A7-37B5984D2615}");

        [Test]
        public async Task GetTeraId_WhenCacheDoesNotExistAndDatabaseEntryDoesNotExist_ReturnsNull()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var outputId = 0L;
            var mockCache = GetDefaultMockCache();
            var manager = GetManager(dbContext, mockCache.Object);
            var id = await manager.GetTeraId(TestTeraId);

            mockCache.Verify(c => c.TryGetTeraId(
                    It.IsAny<Guid>(), out outputId), Times.Once);
            mockCache.VerifyNoOtherCalls();

            Assert.IsNull(id);
        }

        [Test]
        public async Task GetTeraId_WhenCacheDoesNotExistAndDatabaseEntryDoesExist_ReturnsId()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var outputId = 0L;
            var mockCache = GetDefaultMockCache();
            var manager = GetManager(dbContext, mockCache.Object);

            AddTeraAgent(dbContext);

            var id = await manager.GetTeraId(TestTeraId);

            Assert.AreEqual(TestTeraAgentId, id);

            mockCache.Verify(c => c.TryGetTeraId(
                It.IsAny<Guid>(), out outputId), Times.Once);
            mockCache.Verify(c => c.RegisterTeraId(
                It.IsAny<Guid>(), It.IsAny<long>()),
                Times.Once);
            mockCache.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetTeraId_WhenCacheExists_ReturnsId()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var outputId = TestTeraAgentId;
            var mockCache = GetDefaultMockCache();
            var manager = GetManager(dbContext, mockCache.Object);

            mockCache
                .Setup(c => c.TryGetTeraId(It.IsAny<Guid>(), out outputId))
                .Returns(true);

            var id = await manager.GetTeraId(TestTeraId);

            Assert.AreEqual(TestTeraAgentId, id);

            mockCache.Verify(c => c.TryGetTeraId(
                It.IsAny<Guid>(), out outputId), Times.Once);
            mockCache.VerifyNoOtherCalls();
        }

        private static void AddTeraAgent(TeraDbContext dbContext)
        {
            var builder = new MockTeraDbContextBuilder(dbContext);

            builder.AddTeraAgent(TestTeraId, TestTeraAgentId);
        }

        private static Mock<ITeraAgentCache> GetDefaultMockCache()
        {
            var result = new Mock<ITeraAgentCache>();

            return result;
        }

        private static ITeraAgentManager GetManager(
            TeraDbContext dbContext,
            ITeraAgentCache cache)
        {
            return new TeraAgentManager(dbContext, cache);
        }
    }
}
