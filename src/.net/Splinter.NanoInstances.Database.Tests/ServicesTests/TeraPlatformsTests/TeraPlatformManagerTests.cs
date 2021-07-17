using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.TeraPlatforms;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Domain.Settings.TeraPlatforms;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.TeraPlatformsTests
{
    [TestFixture]
    public class TeraPlatformManagerTests
    {
        private const long TestTeraPlatformId = 123;
        private const long TestOperatingSystemId = 100;
        private const string TestFrameworkDescription = "Unit Test Framework";

        private static readonly Guid TestPlatformTeraId = new("{C239227E-BDDB-4282-B593-B4A4A0194CCB}");

        [Test]
        public async Task RegisterTeraPlatform_WhenPlatformDoesNotExist_CreatesANewPlatform()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var manager = GetManager(dbContext);

            AddDefaultData(dbContext);

            var platformId = await manager.RegisterTeraPlatform(TestOperatingSystemId);

            AssertTeraPlatform(dbContext, platformId);
        }

        [Test]
        public async Task RegisterTeraPlatform_WhenPlatformDoesExist_SetsToAsRunning()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var manager = GetManager(dbContext);

            AddDefaultData(dbContext);
            AddDefaultTeraPlatform(dbContext);

            var platformId = await manager.RegisterTeraPlatform(TestOperatingSystemId);

            AssertTeraPlatform(dbContext, platformId);
        }

        [Test]
        public async Task DisposeTeraPlatform_WhenPlatformDoesNotExist_DoesNothing()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var manager = GetManager(dbContext);

            AddDefaultData(dbContext);

            Assert.DoesNotThrowAsync(() => manager.DisableTeraPlatform());
        }

        [Test]
        public async Task DisposeTeraPlatform_WhenPlatformDoesExist_SetsItAsHalted()
        {
            await using var dbContextFactory = new MockTeraDbContextFactory();
            await using var dbContext = dbContextFactory.Context;
            var manager = GetManager(dbContext);

            AddDefaultData(dbContext);
            AddDefaultTeraPlatform(dbContext);

            Assert.DoesNotThrowAsync(() => manager.DisableTeraPlatform());

            AssertTeraPlatform(dbContext, TestTeraPlatformId, TeraPlatformStatus.Halted);
        }

        private static void AddDefaultData(TeraDbContext teraDbContext)
        {
            var builder = new MockTeraDbContextBuilder(teraDbContext);

            builder.AddOperatingSystem(TestOperatingSystemId);
        }

        private static void AddDefaultTeraPlatform(TeraDbContext teraDbContext)
        {
            var builder = new MockTeraDbContextBuilder(teraDbContext);

            builder.AddTeraPlatform(
                TestOperatingSystemId,
                TestTeraPlatformId,
                TestPlatformTeraId,
                TeraPlatformStatus.Running,
                TestFrameworkDescription);
        }

        private static void AssertTeraPlatform(
            TeraDbContext teraDbContext, 
            long platformId,
            TeraPlatformStatus expectedStatus = TeraPlatformStatus.Running)
        {
            var platform = teraDbContext.TeraPlatforms.Single();

            Assert.AreEqual(platformId, platform.Id);
            Assert.AreEqual(TestPlatformTeraId, platform.TeraId);
            Assert.AreEqual(TestOperatingSystemId, platform.OperatingSystemId);
            Assert.AreEqual(TestFrameworkDescription, platform.FrameworkDescription);
            Assert.AreEqual(expectedStatus, platform.Status);
        }

        private static IOperatingSystemInformationProvider GetDefaultOsProvider()
        {
            var result = new Mock<IOperatingSystemInformationProvider>();

            result
                .Setup(p => p.GetFrameworkDescription())
                .ReturnsAsync(TestFrameworkDescription);

            return result.Object;
        }

        private static TeraPlatformManager GetManager(TeraDbContext teraDbContext)
        {
            var settings = new TeraPlatformSettings
            {
                TeraId = TestPlatformTeraId
            };

            return new TeraPlatformManager(settings, teraDbContext, GetDefaultOsProvider());
        }
    }
}
