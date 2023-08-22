using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoInstances.Database.Tests.Utilities;
using Splinter.NanoTypes.Domain.Exceptions.Data;

namespace Splinter.NanoInstances.Database.Tests.ExtensionsTests;

[TestFixture]
public class DbContextExtensionsTests
{
    [Test]
    public async Task GetTeraAgent_WhenTeraAgentIsNotPresent_ThrowsAnError()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var builder = new MockTeraDataBuilder(dbContext);
        var teraId = Guid.NewGuid();
        var findTeraId = Guid.NewGuid();

        builder.AddTeraAgent(teraId);

        var error = Assert.ThrowsAsync<SplinterEntityNotFoundException>(() => dbContext.GetTeraAgent(findTeraId))!;

        error.Should().NotBeNull();
        error.Message.Should().Be($"Could not find the Tera Agent with ID {findTeraId}.");
    }

    [Test]
    public async Task GetTeraAgent_WhenTeraAgentIsPresent_ReturnsTeraAgent()
    {
        await using var dbContextFactory = new MockTeraDbContextFactory();
        await using var dbContext = dbContextFactory.Context;
        var builder = new MockTeraDataBuilder(dbContext);
        var teraId = Guid.NewGuid();

        builder.AddTeraAgent(teraId);

        var teraAgent = await dbContext.GetTeraAgent(teraId);

        teraAgent.Should().NotBeNull();
        teraAgent.Id.Should().Be(1);
        teraAgent.TeraId.Should().Be(teraId);
    }
}