using NUnit.Framework;
using System;
using FluentAssertions;
using Splinter.NanoInstances.Extensions;

namespace Splinter.NanoInstances.Tests.ExtensionsTests;

public class NanoTypeExtensionsTests
{
    [Test]
    public void ToSplinterId_GivenGuid_ReturnsAnEmptySplinterId()
    {
        var guid = Guid.NewGuid();
        var result = guid.ToSplinterId();

        result.Should().NotBeNull();
        result.Name.Should().Be(guid.ToString());
        result.Version.Should().Be("0");
        result.Guid.Should().Be(guid);
    }
}
