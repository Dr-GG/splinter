using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Superposition;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.SuperpositionTests
{
    [TestFixture]
    public class NanoTypeRecollapseOperationServiceTests
    {
        private const long TestNanoTypeId = 22;
        private const long TestNumberOfSuccessfulRecollapses = 421;
        private const long TestNumberOfFailedRecollapses = 13;

        private static readonly Guid TestOperationGuid = new("{8128DD10-E80D-45AA-9520-C0A7A2FC4F03}");
        private static readonly Guid TestTeraId = new("{B8D6FE74-F46B-4590-8B34-0A2B641982B7}");

        [Test]
        public async Task Sync_WhenOperationDoesNotExists_DoesNotThrowAnError()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new NanoRecollapseOperationParameters
            {
                NanoRecollapseOperationId = TestOperationGuid
            };

            Assert.DoesNotThrowAsync(() => service.Sync(parameters));
        }

        [Test]
        public async Task Sync_WhenOperationWasSuccessful_IncrementsSuccessfulCount()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new NanoRecollapseOperationParameters
            {
                IsSuccessful = true,
                TeraId = TestTeraId,
                NanoRecollapseOperationId = TestOperationGuid
            };

            AddDefaultData(dbContext);

            await service.Sync(parameters);

            var operation = dbContext.NanoTypeRecollapseOperations.Single();

            Assert.AreEqual(TestOperationGuid, operation.Guid);
            Assert.AreEqual(TestNumberOfFailedRecollapses, operation.NumberOfFailedRecollapses);
            Assert.AreEqual(TestNumberOfSuccessfulRecollapses + 1, operation.NumberOfSuccessfulRecollapses);
        }

        [Test]
        public async Task Sync_WhenOperationFailed_IncrementsFailedCount()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var service = GetService(dbContext);
            var parameters = new NanoRecollapseOperationParameters
            {
                IsSuccessful = false,
                TeraId = TestTeraId,
                NanoRecollapseOperationId = TestOperationGuid
            };

            AddDefaultData(dbContext);

            await service.Sync(parameters);

            var operation = dbContext.NanoTypeRecollapseOperations.Single();

            Assert.AreEqual(TestOperationGuid, operation.Guid);
            Assert.AreEqual(TestNumberOfFailedRecollapses + 1, operation.NumberOfFailedRecollapses);
            Assert.AreEqual(TestNumberOfSuccessfulRecollapses, operation.NumberOfSuccessfulRecollapses);
        }

        private static void AddDefaultData(TeraDbContext dbContext)
        {
            var builder = new MockTeraDbContextBuilder(dbContext);

            builder.AddNanoType(TestNanoTypeId);
            builder.AddNanoTypeRecollapseOperation(TestNanoTypeId, 
                guid: TestOperationGuid,
                numberOfFailedRecollapses: TestNumberOfFailedRecollapses,
                numberOfSuccessfulRecollapses: TestNumberOfSuccessfulRecollapses);
        }

        private static INanoTypeRecollapseOperationService GetService(TeraDbContext dbContext)
        {
            return new NanoTypeRecollapseOperationService(dbContext);
        }
    }
}
