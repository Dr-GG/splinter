using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoInstances.Database.Services.NanoTypes;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.NanoTypesTests;

[TestFixture]
public class TeraAgentManagerTests
{
    private const long TestNewNanoTypeId = 1;
    private const long TestExistingNanoTypeId = 445;
    private const long TestNewNanoInstanceId = 1;
    private const long TestExistingNanoInstanceId = 446;

    private static readonly SplinterId TestNanoTypeId = new()
    {
        Guid = new Guid("{0B92A6EC-08C3-40BE-B163-9BC76856D82D}"),
        Name = "Test Nano Type Id",
        Version = "1.0.0.1"
    };

    private static readonly SplinterId TestNanoInstanceId = new()
    {
        Guid = new Guid("{C3EA0883-2F4A-44F1-A864-E9221F2DBF62}"),
        Name = "Test Nano Instance Id",
        Version = "2.0.0.1"
    };

    [Test]
    public async Task RegisterNanoType_WhenNanoTypeIsNotInCacheOrDatabase_AddsIt()
    {
        long dummyId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var agent = GetMockNanoAgent();
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        await manager.RegisterNanoType(agent);

        var nanoType = dbContext.NanoTypes.Single();

        AssertNanoType(nanoType, TestNewNanoTypeId);

        mockCache
            .Verify(m => m
                    .TryGetNanoTypeId(It.IsAny<SplinterId>(), out dummyId),
                Times.Once);

        mockCache
            .Verify(m => m
                    .RegisterNanoTypeId(
                        It.IsAny<SplinterId>(), 
                        It.IsAny<long>()),
                Times.Once);
    }

    [Test]
    public async Task RegisterNanoType_WhenNanoTypeIsNotInCacheAndIsInDatabase_DoesNotAddData()
    {
        long dummyId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var agent = GetMockNanoAgent();
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        AddNanoTypeData(dbContext);

        await manager.RegisterNanoType(agent);

        var nanoType = dbContext.NanoTypes.Single();

        AssertNanoType(nanoType, TestExistingNanoTypeId);

        mockCache
            .Verify(m => m
                    .TryGetNanoTypeId(It.IsAny<SplinterId>(), out dummyId),
                Times.Once);

        mockCache
            .Verify(m => m
                    .RegisterNanoTypeId(
                        It.IsAny<SplinterId>(),
                        It.IsAny<long>()),
                Times.Once);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoTypeIsNotInCacheAndIsInDatabase_ReturnsTheDatabaseId()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        AddNanoTypeData(dbContext);

        var nanoTypeId = await manager.GetNanoTypeId(TestNanoTypeId);

        Assert.AreEqual(TestExistingNanoTypeId, nanoTypeId);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoTypeIsInCache_ReturnsCacheId()
    {
        var id = TestExistingNanoTypeId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        mockCache
            .Setup(m => m.TryGetNanoTypeId(
                It.IsAny<SplinterId>(), out id))
            .Returns(true);

        var nanoTypeId = await manager.GetNanoTypeId(TestNanoTypeId);

        Assert.AreEqual(TestExistingNanoTypeId, nanoTypeId);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoTypeIsNotInCacheOrDatabase_ReturnsNull()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);
        var nanoTypeId = await manager.GetNanoTypeId(TestNanoTypeId);

        Assert.IsNull(nanoTypeId);
    }

    [Test]
    public async Task RegisterNanoType_WhenNanoInstanceIsNotInCacheOrDatabase_AddsIt()
    {
        long dummyId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var agent = GetMockNanoAgent();
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        await manager.RegisterNanoType(agent);

        var nanoType = dbContext.NanoTypes.Single();

        AssertNanoType(nanoType, TestNewNanoInstanceId);

        mockCache
            .Verify(m => m
                    .TryGetNanoInstanceId(It.IsAny<SplinterId>(), out dummyId),
                Times.Once);

        mockCache.Verify(m => m
                .RegisterNanoInstanceId(
                    It.IsAny<SplinterId>(),
                    It.IsAny<long>()),
            Times.Once);
    }

    [Test]
    public async Task RegisterNanoType_WhenNanoInstanceIsNotInCacheAndIsInDatabase_DoesNotAddData()
    {
        long dummyId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var agent = GetMockNanoAgent();
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        AddNanoTypeData(dbContext);
        AddNanoInstanceData(dbContext);

        await manager.RegisterNanoType(agent);

        var nanoInstance = dbContext.NanoInstances.Single();

        AssertNanoInstance(nanoInstance, TestExistingNanoInstanceId);

        mockCache
            .Verify(m => m
                    .TryGetNanoInstanceId(It.IsAny<SplinterId>(), out dummyId),
                Times.Once);

        mockCache
            .Verify(m => m
                    .RegisterNanoInstanceId(
                        It.IsAny<SplinterId>(),
                        It.IsAny<long>()),
                Times.Once);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoInstanceIsNotInCacheAndIsInDatabase_ReturnsTheDatabaseId()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        AddNanoTypeData(dbContext);
        AddNanoInstanceData(dbContext);

        var nanoInstanceId = await manager.GetNanoInstanceId(TestNanoInstanceId);

        Assert.AreEqual(TestExistingNanoInstanceId, nanoInstanceId);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoInstanceIsNotInCacheOrDatabase_ReturnsNull()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);
        var nanoInstanceId = await manager.GetNanoInstanceId(TestNanoTypeId);

        Assert.IsNull(nanoInstanceId);
    }

    [Test]
    public async Task GetNanoTypeId_WhenNanoInstanceIsInCache_ReturnsCacheId()
    {
        var id = TestExistingNanoInstanceId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        mockCache
            .Setup(m => m.TryGetNanoInstanceId(
                It.IsAny<SplinterId>(), out id))
            .Returns(true);

        var nanoInstanceId = await manager.GetNanoInstanceId(TestNanoTypeId);

        Assert.AreEqual(TestExistingNanoInstanceId, nanoInstanceId);
    }

    [Test]
    public async Task RegisterNanoType_WhenNanoTypeAndNanoInstanceIsInCache_SkipsAddingItToTheDatabase()
    {
        var outTypeId = TestExistingNanoTypeId;
        var outInstanceId = TestExistingNanoInstanceId;
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var agent = GetMockNanoAgent();
        var mockCache = GetDefaultMockCache();
        var manager = GetManager(dbContext, mockCache.Object);

        mockCache
            .Setup(m => m.TryGetNanoTypeId(
                It.IsAny<SplinterId>(), out outTypeId))
            .Returns(true);

        mockCache
            .Setup(m => m.TryGetNanoInstanceId(
                It.IsAny<SplinterId>(), out outInstanceId))
            .Returns(true);

        await manager.RegisterNanoType(agent);

        Assert.IsEmpty(dbContext.NanoTypes);

        mockCache
            .Verify(m => m
                    .TryGetNanoTypeId(It.IsAny<SplinterId>(), out outTypeId),
                Times.Once);

        mockCache
            .Verify(m => m
                    .TryGetNanoInstanceId(It.IsAny<SplinterId>(), out outInstanceId),
                Times.Once);

        mockCache
            .Verify(m => m
                    .RegisterNanoTypeId(
                        It.IsAny<SplinterId>(),
                        It.IsAny<long>()),
                Times.Never);

        mockCache
            .Verify(m => m
                    .RegisterNanoInstanceId(
                        It.IsAny<SplinterId>(),
                        It.IsAny<long>()),
                Times.Never);
    }

    private static void AssertNanoType(NanoTypeModel nanoType, long expectedId)
    {
        Assert.AreEqual(expectedId, nanoType.Id);
        Assert.AreEqual(TestNanoTypeId.Guid, nanoType.Guid);
        Assert.AreEqual(TestNanoTypeId.Name, nanoType.Name);
        Assert.AreEqual(TestNanoTypeId.Version, nanoType.Version);
    }

    private static void AssertNanoInstance(NanoInstanceModel nanoInstance, long expectedId)
    {
        Assert.AreEqual(expectedId, nanoInstance.Id);
        Assert.AreEqual(TestNanoInstanceId.Guid, nanoInstance.Guid);
        Assert.AreEqual(TestNanoInstanceId.Name, nanoInstance.Name);
        Assert.AreEqual(TestNanoInstanceId.Version, nanoInstance.Version);
    }

    private static void AddNanoTypeData(TeraDbContext teraDbContext)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);

        builder.AddNanoType(
            TestExistingNanoTypeId,
            TestNanoTypeId.Guid,
            TestNanoTypeId.Name,
            TestNanoTypeId.Version);
    }

    private static void AddNanoInstanceData(TeraDbContext teraDbContext)
    {
        var builder = new MockTeraDataBuilder(teraDbContext);

        builder.AddNanoInstance(
            TestExistingNanoTypeId,
            TestExistingNanoInstanceId,
            TestNanoInstanceId.Guid,
            TestNanoInstanceId.Name,
            TestNanoInstanceId.Version);
    }

    private static Mock<INanoTypeCache> GetDefaultMockCache()
    {
        var result = new Mock<INanoTypeCache>();

        return result;
    }

    private static INanoAgent GetMockNanoAgent()
    {
        return GetMockNanoAgent(TestNanoTypeId, TestNanoInstanceId);
    }

    private static INanoAgent GetMockNanoAgent(
        SplinterId nanoTypeId,
        SplinterId nanoInstanceId)
    {
        var mockAgent = new Mock<INanoAgent>();

        mockAgent
            .Setup(m => m.TypeId)
            .Returns(nanoTypeId);

        mockAgent
            .Setup(m => m.InstanceId)
            .Returns(nanoInstanceId);

        return mockAgent.Object;
    }

    private static INanoTypeManager GetManager(
        TeraDbContext teraDbContext,
        INanoTypeCache cache)
    {
        return new NanoTypeManager(teraDbContext, cache);
    }
}