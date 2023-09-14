using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Default.Services.NanoTypes;
using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Tests.ServicesTests.NanoTypesTests;

public class NanoTypeCacheTests
{
    [Test]
    public void TryGetNanoTypeId_WhenNanoTypeIdIsNotRegistered_ReturnsFalse()
    {
        var cache = GetDefaultCache();
        var result = cache.TryGetNanoTypeId(GetRandomSplinterId(), out var id);

        result.Should().BeFalse();
        id.Should().Be(0);
    }

    [Test]
    public void TryGetNanoInstanceId_WhenNanoInstanceIdIsNotRegistered_ReturnsFalse()
    {
        var cache = GetDefaultCache();
        var result = cache.TryGetNanoInstanceId(GetRandomSplinterId(), out var id);

        result.Should().BeFalse();
        id.Should().Be(0);
    }

    [Test]
    public void TryGetNanoTypeId_WhenNanoTypeIdIsRegistered_ReturnsTrue()
    {
        var splinterId = GetRandomSplinterId();
        var cache = GetDefaultCache();
        var id = DateTime.Now.Ticks;

        cache.RegisterNanoTypeId(splinterId, id);

        var result = cache.TryGetNanoTypeId(splinterId, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);
    }

    [Test]
    public void TryGetNanoInstanceId_WhenNanoInstanceIdIsRegistered_ReturnsTrue()
    {
        var splinterId = GetRandomSplinterId();
        var cache = GetDefaultCache();
        var id = DateTime.Now.Ticks;

        cache.RegisterNanoInstanceId(splinterId, id);

        var result = cache.TryGetNanoInstanceId(splinterId, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);
    }

    [Test]
    public async Task TryGetNanoTypeId_WhenSliderExpirationExpires_ReturnsFalse()
    {
        var splinterId = GetRandomSplinterId();
        var cache = GetDefaultCache(new NanoTypeCacheSettings{SlidingExpirationTimespan = TimeSpan.FromSeconds(3)});
        var id = DateTime.Now.Ticks;

        cache.RegisterNanoTypeId(splinterId, id);

        var result = cache.TryGetNanoTypeId(splinterId, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);

        await Task.Delay(TimeSpan.FromSeconds(5));

        result = cache.TryGetNanoTypeId(splinterId, out resultId);

        result.Should().BeFalse();
        resultId.Should().Be(0);
    }

    [Test]
    public async Task TryGetNanoInstanceId_WhenSliderExpirationExpires_ReturnsFalse()
    {
        var splinterId = GetRandomSplinterId();
        var cache = GetDefaultCache(new NanoTypeCacheSettings { SlidingExpirationTimespan = TimeSpan.FromSeconds(3) });
        var id = DateTime.Now.Ticks;

        cache.RegisterNanoInstanceId(splinterId, id);

        var result = cache.TryGetNanoInstanceId(splinterId, out var resultId);

        result.Should().BeTrue();
        resultId.Should().Be(id);

        await Task.Delay(TimeSpan.FromSeconds(5));

        result = cache.TryGetNanoInstanceId(splinterId, out resultId);

        result.Should().BeFalse();
        resultId.Should().Be(0);
    }

    private static SplinterId GetRandomSplinterId()
    {
        return new SplinterId
        {
            Guid = Guid.NewGuid(),
            Name = "Random",
            Version = "rnd"
        };
    }

    private static INanoTypeCache GetDefaultCache(NanoTypeCacheSettings? settings = null)
    {
        settings ??= new NanoTypeCacheSettings
        {
            SlidingExpirationTimespan = TimeSpan.FromMinutes(5)
        };

        return new NanoTypeCache(settings);
    }
}
