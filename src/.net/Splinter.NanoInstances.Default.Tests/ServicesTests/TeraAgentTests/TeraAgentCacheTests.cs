using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.TeraAgents;
using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.TeraAgentTests;

public class TeraAgentCacheTests
{
    [Test]
    public void TryGetTeraId_WhenRetrievingIdThatDoesNotExists_ReturnsFalse()
    {
        var cache = GetDefaultCache();
        var result = cache.TryGetTeraId(Guid.NewGuid(), out var id);

        result.Should().BeFalse();
        id.Should().Be(0);
    }

    [Test]
    public void TryGetTeraId_WhenRetrievingIdThatDoesExist_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        var id = DateTime.Now.Ticks;
        var cache = GetDefaultCache();

        cache.RegisterTeraId(guid, id);

        var result = cache.TryGetTeraId(guid, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);
    }

    [Test]
    public void RegisterTeraId_WhenRegisterTheSameGuidMultipleTimes_StillWorks()
    {
        var guid = Guid.NewGuid();
        var id = DateTime.Now.Ticks;
        var cache = GetDefaultCache();

        cache.RegisterTeraId(guid, id);
        cache.RegisterTeraId(guid, id);

        var result = cache.TryGetTeraId(guid, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);
    }

    [Test]
    public void RegisterTeraId_ProvidingSlidingExpirationTimespan_EnforcesSliderExpiration()
    {
        var guid = Guid.NewGuid();
        var id = DateTime.Now.Ticks;
        var cache = GetDefaultCache(new TeraAgentCacheSettings
        {
            SlidingExpirationTimespan = TimeSpan.FromSeconds(5)
        });

        cache.RegisterTeraId(guid, id);

        var result = cache.TryGetTeraId(guid, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);

        Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

        result = cache.TryGetTeraId(guid, out resultId);

        result.Should().BeFalse();
        resultId.Should().Be(0);
    }

    private static ITeraAgentCache GetDefaultCache(TeraAgentCacheSettings? settings = null)
    {
        settings ??= new TeraAgentCacheSettings
        {
            SlidingExpirationTimespan = TimeSpan.FromMinutes(1)
        };

        return new TeraAgentCache(settings);
    }
}
