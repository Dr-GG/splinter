using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Splinter.Applications.Test.Domain.Constants;
using Splinter.Applications.Test.Utilities;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Environment;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.Applications.Test.Tests;

public static class TestTeraAgentLifecycleTests
{
    private const int NumberOfTeraAgents = 100;

    public static async Task Test()
    {
        TestUtilities.SectionText("TERA AGENT LIFECYCLE TESTS");

        await TestUtilities.ExecuteTest("Testing Tera Agent lifecycle of Tera Agents with random GUIDs", TestRandomGuidTeraAgentLifecycle);
        await TestUtilities.ExecuteTest("Testing Tera Agent lifecycle of Tera Agents with fixed GUID", TestFixedGuidTeraAgentLifecycle);
    }

    private static async Task TestRandomGuidTeraAgentLifecycle()
    {
        var teraIds = new List<Guid>();

        for (var i = 0; i < NumberOfTeraAgents; i++)
        {
            var agent = await InitialiseTeraAgent();

            teraIds.Add(agent.TeraId);

            await AssertAsRunning(agent.TeraId);
            await agent.Terminate();
            await AssertAsDisposed(agent.TeraId);
        }

        teraIds.Distinct().Should().HaveCount(NumberOfTeraAgents);
    }

    private static async Task TestFixedGuidTeraAgentLifecycle()
    {
        var teraId = Guid.NewGuid();

        for (var i = 0; i < NumberOfTeraAgents; i++)
        {
            var agent = await InitialiseTeraAgent(teraId);

            await AssertAsRunning(agent.TeraId);
            await agent.Terminate();
            await AssertAsDisposed(agent.TeraId);
        }
    }

    private static async Task AssertAsRunning(Guid teraId)
    {
        await using var scope = await SplinterEnvironment.SuperpositionAgent.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var teraAgent = (await dbContext.TeraAgents.SingleOrDefaultAsync(a => a.TeraId == teraId))!;

        teraAgent.Should().NotBeNull();
        teraAgent.Status.Should().Be(TeraAgentStatus.Running);
    }

    private static async Task AssertAsDisposed(Guid teraId)
    {
        await using var scope = await SplinterEnvironment.SuperpositionAgent.Scope.Start();
        await using var dbContext = await scope.Resolve<TeraDbContext>();
        var teraAgent = (await dbContext.TeraAgents.SingleOrDefaultAsync(a => a.TeraId == teraId))!;

        teraAgent.Should().NotBeNull();
        teraAgent.Status.Should().Be(TeraAgentStatus.Disposed);
    }

    private static async Task<ITeraAgent> InitialiseTeraAgent(Guid? teraId = null)
    {
        var collapse = new NanoCollapseParameters
        {
            NanoTypeId = TeraMessageSplinterIds.TeraTypeId.Guid,
            TeraAgentId = teraId
        };
        var result = (await SplinterEnvironment.SuperpositionAgent.Collapse(collapse) as ITeraAgent)!;

        result.Should().NotBeNull();

        var init = new NanoInitialisationParameters();

        await result.Initialise(init);

        return result;
    }
}