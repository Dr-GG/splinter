using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Domain.Exceptions.Data;

namespace Splinter.NanoInstances.Database.Tests.ExtensionsTests
{
    [TestFixture]
    public class DbContextExtensionsTests
    {
        [Test]
        public async Task GetTeraAgent_WhenTeraAgentIsNotPresent_ThrowsAnError()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var builder = new MockTeraDbContextBuilder(dbContext);
            var teraId = Guid.NewGuid();

            builder.AddTeraAgent(teraId);

            Assert.ThrowsAsync<EntityNotFoundException>(() => dbContext.GetTeraAgent(Guid.NewGuid()));
        }

        [Test]
        public async Task GetTeraAgent_WhenTeraAgentIsPresent_ReturnsTeraAgent()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var builder = new MockTeraDbContextBuilder(dbContext);
            var teraId = Guid.NewGuid();

            builder.AddTeraAgent(teraId);

            var teraAgent = await dbContext.GetTeraAgent(teraId);

            Assert.IsNotNull(teraAgent);
            Assert.AreEqual(1, teraAgent.Id);
            Assert.AreEqual(teraId, teraAgent.TeraId);
        }
    }
}
