using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Services.Superposition;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Domain.Messaging.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Messaging;

namespace Splinter.NanoInstances.Database.Tests.ServicesTests.SuperpositionTests;

[TestFixture]
public class TeraAgentNanoTypeDependencyServiceTests
{
    private static readonly Guid TestSourceTeraId = new("{AFCBF207-F719-436C-81A7-E53B4B1C4C05}");
    private static readonly Guid TestTeraId = new("{85F1294D-858D-4113-AD91-0D4F9B5AEC9F}");
    private static readonly SplinterId TestNanoType = new()
    {
        Name = "Test",
        Version = "0",
        Guid = new Guid("{47FD9903-3D33-4A6E-AF43-384508124136}")
    };
    private static readonly SplinterId TestRecollapseNanoType = new()
    {
        Name = "Recollapse Test",
        Version = "1",
        Guid = new Guid("{1D06EEBE-0750-4765-9725-4095BD2DBC1E}")
    };

    private const long TestSourceTeraAgentId = 333;
    private const long TestPlatformId = 221;
    private const long TestTeraAgentId = 123;
    private const long TestRecollapseNanoTypeId = 3211;
    private const long TestNanoTypeId = 321;
    private const long TestNanoInstanceId = 331;
    private const long TestRecollapseNanoInstanceId = 3313;

    private static readonly IEnumerable<Guid> TestRecollapseTestTeraIds = new[]
    {
        new Guid("{BAE4A00C-D95B-45B7-AD44-0936FBCE577D}"),
        new Guid("{23F7FB36-EA4E-486F-8A04-12CD3B50E685}"),
        new Guid("{39E3E3A4-0D21-4089-B6D2-1BC39B9E6412}"),
        new Guid("{A8E20B30-BDE6-45C4-8D3C-AD580964FD13}"),
        new Guid("{BC624A87-0DE3-4B81-B0AD-AF70EDABC233}"),
        new Guid("{F1E1C26C-8F67-41A6-8AC3-DA3985A16F46}"),
        new Guid("{72433D56-E506-48AC-A442-D55CF38092A1}"),
        new Guid("{75F685D9-3902-4335-B0BB-FFEE6B816437}"),
        new Guid("{A1ADFF81-E84D-43F1-89C4-8DE3E8DA2B3C}"),
        new Guid("{B98146FD-1CED-40C3-98AC-832EDB50C536}")
    };

    private static readonly IEnumerable<long> TestRecollapseTestTeraAgentIds = new[]
    {
        834736L,
        37171L,
        393918L,
        531L,
        9986L,
        77776L,
        12230L,
        4884877L,
        388192L,
        2229119L
    };

    [TestCase(null, null)]
    [TestCase(TestTeraAgentId, null)]
    [TestCase(null, TestNanoTypeId)]
    public async Task IncrementTeraAgentNanoTypeDependencies_WhenTeraAgentOrNanoTypeDoesNotExist_DoesNothing(
        long? teraAgentId,
        long? nanoTypeId)
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(teraAgentId);
        var nanoManager = GetMockNanoTypeManager(nanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        await service.IncrementTeraAgentNanoTypeDependencies(TestTeraId, TestNanoType);
        
        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 1)]
    [TestCase(1, 100)]
    public async Task
        IncrementTeraAgentNanoTypeDependencies_WhenDataExistsAndDependencyModelExists_AppendsTheDependencies(
            int initialDependencies, int numberOfDependencies)
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext, initialDependencies);

        await service.IncrementTeraAgentNanoTypeDependencies(TestTeraId, TestNanoType, numberOfDependencies);

        AssertDependency(dbContext, initialDependencies + numberOfDependencies);
    }

    [TestCase(null, null)]
    [TestCase(TestTeraAgentId, null)]
    [TestCase(null, TestNanoTypeId)]
    public async Task DecrementTeraAgentNanoTypeDependencies_WhenTeraAgentOrNanoTypeDoesNotExist_DoesNothing(
        long? teraAgentId,
        long? nanoTypeId)
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(teraAgentId);
        var nanoManager = GetMockNanoTypeManager(nanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        await service.DecrementTeraAgentNanoTypeDependencies(TestTeraId, TestNanoType);

        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [Test]
    public async Task
        DecrementTeraAgentNanoTypeDependencies_WhenDataExistsAndNoDependencyExists_UpdatesNoData()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext);

        await service.DecrementTeraAgentNanoTypeDependencies(TestTeraId, TestNanoType);
        
        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [TestCase(0, 0, 0)]
    [TestCase(0, 10, 0)]
    [TestCase(5, 5, 0)]
    [TestCase(50, 100, 0)]
    [TestCase(50, 25, 25)]
    public async Task
        DecrementTeraAgentNanoTypeDependencies_WhenDataExistsAndDependencyModelExists_AppendsTheDependencies(
            int initialDependencies, 
            int numberOfDependencies, 
            int expectedNumberOfDependencies)
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext, initialDependencies);

        await service.DecrementTeraAgentNanoTypeDependencies(TestTeraId, TestNanoType, numberOfDependencies);

        AssertDependency(dbContext, expectedNumberOfDependencies);
    }

    [Test]
    public async Task DisposeTeraAgentNanoTypeDependencies_WhenTeraAgentDoesNotExists_CreatesNoData()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        await service.DisposeTeraAgentNanoTypeDependencies(TestTeraId);
        
        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [Test]
    public async Task DisposeTeraAgentNanoTypeDependencies_WhenTeraAgentExistsWithNoDependency_CreatesNoData()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext);

        await service.DisposeTeraAgentNanoTypeDependencies(TestTeraId);
        
        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [Test]
    public async Task DisposeTeraAgentNanoTypeDependencies_WhenTeraAgentExistsWithDependency_RemovesTheData()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext, 100);

        await service.DisposeTeraAgentNanoTypeDependencies(TestTeraId);
        
        dbContext.TeraAgentNanoTypeDependencies.Should().BeEmpty();
    }

    [Test]
    public async Task SignalNanoTypeRecollapses_WhenNanoTypeDoesNotExist_ReturnsNull()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager();
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext);

        var operation = await service.SignalNanoTypeRecollapses(
            TestSourceTeraId, TestRecollapseNanoType.Guid, null);
        
        operation.Should().BeNull();
        dbContext.NanoTypeRecollapseOperations.Should().BeEmpty();
    }

    [Test]
    public async Task SignalNanoTypeRecollapses_WhenNoDependenciesExist_ReturnsNull()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestRecollapseNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext);

        var operation = await service.SignalNanoTypeRecollapses(
            TestSourceTeraId, TestRecollapseNanoType.Guid, null);
        
        operation.Should().BeNull();
        dbContext.NanoTypeRecollapseOperations.Should().BeEmpty();
    }

    [Test]
    public async Task SignalNanoTypeRecollapses_WhenDataExistsAndNoTeraIdsAreSelected_RelaysMessagesToAllTeraAgents()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestRecollapseNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);

        AddDefaultData(dbContext, addRecollapseData: true);

        var operationId = await service.SignalNanoTypeRecollapses(
            TestSourceTeraId, TestRecollapseNanoType.Guid, null);
        var operation = dbContext.NanoTypeRecollapseOperations.Single();
        
        operationId.Should().NotBeNull();
        operation.Guid.Should().Be(operationId!.Value);

        var numberOfDependencies = AssertRecollapseSignals(dbContext, operationId.Value, TestRecollapseTestTeraAgentIds);

        operation.NumberOfExpectedRecollapses.Should().Be(numberOfDependencies);
        operation.NumberOfFailedRecollapses.Should().Be(0);
        operation.NumberOfSuccessfulRecollapses.Should().Be(0);
    }

    [Test]
    public async Task SignalNanoTypeRecollapses_WhenDataExistsAndSpecificTeraIdsAreProvided_RelaysMessagesToAllTeraAgents()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var teraManager = GetMockTeraAgentManager(TestTeraAgentId);
        var nanoManager = GetMockNanoTypeManager(TestRecollapseNanoTypeId);
        var service = GetService(dbContext, teraManager, nanoManager);
        var teraIds = TestRecollapseTestTeraIds.Skip(9).ToList();
        var teraAgentIds = TestRecollapseTestTeraAgentIds.Skip(9).ToList();

        AddDefaultData(dbContext, addRecollapseData: true);

        var operationId = await service.SignalNanoTypeRecollapses(
            TestSourceTeraId, TestRecollapseNanoType.Guid, teraIds);
        var operation = dbContext.NanoTypeRecollapseOperations.Single();

        operationId.Should().NotBeNull();
        operation.Guid.Should().Be(operationId!.Value);

        var numberOfDependencies = AssertRecollapseSignals(dbContext, operationId.Value, teraAgentIds);

        operation.NumberOfExpectedRecollapses.Should().Be(numberOfDependencies);
        operation.NumberOfFailedRecollapses.Should().Be(0);
        operation.NumberOfSuccessfulRecollapses.Should().Be(0);
    }

    private static void AddDefaultData(
        TeraDbContext dbContext, 
        int? numberOfDependencies = null,
        bool addRecollapseData = false)
    {
        var builder = new MockTeraDataBuilder(dbContext);

        builder.AddTeraPlatform(TestPlatformId);
        builder.AddNanoType(TestNanoTypeId, TestNanoType.Guid);
        builder.AddNanoType(TestRecollapseNanoTypeId, TestRecollapseNanoType.Guid);
        builder.AddNanoInstance(TestNanoTypeId, TestNanoInstanceId);
        builder.AddNanoInstance(TestRecollapseNanoTypeId, TestRecollapseNanoInstanceId);
        builder.AddTeraAgent(TestPlatformId, TestNanoInstanceId, TestTeraAgentId);
        builder.AddTeraAgent(TestPlatformId, TestNanoInstanceId, TestSourceTeraAgentId, TestSourceTeraId);

        if (numberOfDependencies.HasValue)
        {
            builder.AddTeraAgentNanoTypeDependency(
                TestTeraAgentId,
                TestNanoTypeId,
                numberOfDependencies: numberOfDependencies);
        }

        if (addRecollapseData)
        {
            AddRecollapseTeraAgents(dbContext);
        }
    }

    private static void AddRecollapseTeraAgents(TeraDbContext dbContext)
    {
        var enumeratedTeraAgentIds = TestRecollapseTestTeraAgentIds.ToList();
        var enumeratedTeraIds = TestRecollapseTestTeraIds.ToList();
        var builder = new MockTeraDataBuilder(dbContext);

        for (var i = 0; i < enumeratedTeraIds.Count; i++)
        {
            var teraAgentId = enumeratedTeraAgentIds[i];
            var teraId = enumeratedTeraIds[i];

            builder.AddTeraAgent(TestPlatformId, TestNanoInstanceId, teraAgentId, teraId);
            builder.AddTeraAgentNanoTypeDependency(teraAgentId, TestRecollapseNanoTypeId, numberOfDependencies: (int)teraAgentId);
        }
    }

    private static void AssertDependency(TeraDbContext dbContext, int numberOfDependencies)
    {
        var dependency = dbContext.TeraAgentNanoTypeDependencies.First();

        dependency.TeraAgentId.Should().Be(TestTeraAgentId);
        dependency.NanoTypeId.Should().Be(TestNanoTypeId);
        dependency.NumberOfDependencies.Should().Be(numberOfDependencies);
    }

    private static long AssertRecollapseSignals(TeraDbContext dbContext, Guid operationId, IEnumerable<long> teraIds)
    {
        return teraIds.Sum(id => AssertRecollapseSignal(dbContext, operationId, id));
    }

    private static long AssertRecollapseSignal(TeraDbContext dbContext, Guid operationId, long teraId)
    {
        var message = dbContext.TeraMessages.Single(m => m.RecipientTeraAgentId == teraId);
        var dependency = dbContext.TeraAgentNanoTypeDependencies.SingleOrDefault(d => d.TeraAgentId == teraId);
        var data = JsonSerializer.Deserialize<RecollapseTeraMessageData>(message.Message!, JsonConstants.DefaultOptions)!;

        dependency.Should().NotBeNull();
        message.Code.Should().Be(TeraMessageCodeConstants.Recollapse);
        message.Priority.Should().Be(int.MaxValue);
        data.NanoTypeRecollapseOperationId.Should().Be(operationId);
        data.NanoTypeId.Should().Be(TestRecollapseNanoType.Guid);

        return 1;
    }

    private static ITeraAgentManager GetMockTeraAgentManager(long? teraId = null)
    {
        var result = new Mock<ITeraAgentManager>();

        result
            .Setup(m => m.GetTeraId(It.IsAny<Guid>()))
            .ReturnsAsync(teraId);

        return result.Object;
    }

    private static INanoTypeManager GetMockNanoTypeManager(long? nanoTypeId = null)
    {
        var result = new Mock<INanoTypeManager>();

        result
            .Setup(m => m.GetNanoTypeId(It.IsAny<SplinterId>()))
            .ReturnsAsync(nanoTypeId);

        return result.Object;
    }

    private static ITeraMessageRelayService GetMockRelayService(TeraDbContext dbContext)
    {
        var result = new Mock<ITeraMessageRelayService>();

        result
            .Setup(t => t.Relay(It.IsAny<TeraMessageRelayParameters>()))
            .Callback((TeraMessageRelayParameters parameters) =>
            {
                var builder = new MockTeraDataBuilder(dbContext);

                parameters.RecipientTeraIds
                    .ToList()
                    .ForEach(teraId =>
                    {
                        var recipientTeraIdIndex = TestRecollapseTestTeraIds
                            .ToList()
                            .IndexOf(teraId);
                        var teraAgentId = TestRecollapseTestTeraAgentIds.ElementAt(recipientTeraIdIndex);

                        builder.AddTeraMessage(
                            TestSourceTeraAgentId,
                            teraAgentId,
                            priority: parameters.Priority,
                            code: parameters.Code,
                            message: parameters.Message);
                    });
            });

        return result.Object;
    }

    private static ITeraAgentNanoTypeDependencyService GetService(
        TeraDbContext dbContext,
        ITeraAgentManager teraAgentManager,
        INanoTypeManager nanoTypeManager)
    {
        return new TeraAgentNanoTypeDependencyService(
            dbContext,
            nanoTypeManager,
            teraAgentManager,
            GetMockRelayService(dbContext));
    }
}