using FluentAssertions;
using Moq;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Exceptions.Data;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Tests.ExtensionsTests;

public class ObjectExtensionsTests
{
    [Test]
    public void AssertNanoTypeReturnGetterValue_WhenValueIsNull_ThrowsException()
    {
        INanoAgent? value = null;

        var error = Assert.Throws<NanoTypeNotInitialiseException<INanoAgent>>(() => value.AssertNanoTypeReturnGetterValue());

        error.Should().NotBeNull();
    }

    [Test]
    public void AssertNanoTypeReturnGetterValue_WhenValueIsNotNull_ReturnsTheOriginalValue()
    {
        var value = new Mock<INanoAgent>().Object;
        var response = value.AssertNanoTypeReturnGetterValue();

        response.Should().BeSameAs(value);
    }

    [Test]
    public void AssertNanoServiceReturnGetterValue_WhenValueIsNull_ThrowsException()
    {
        INanoTable? value = null;

        var error = Assert.Throws<NanoServiceNotInitialiseException<INanoTable>>(() => value.AssertNanoServiceReturnGetterValue());

        error.Should().NotBeNull();
    }

    [Test]
    public void AssertNanoServiceReturnGetterValue_WhenValueIsNotNull_ReturnsTheOriginalValue()
    {
        var value = new Mock<INanoTable>().Object;
        var response = value.AssertNanoServiceReturnGetterValue();

        response.Should().BeSameAs(value);
    }

    [Test]
    public void AssertReturnGetterValue_WhenValueIsNull_ThrowsException()
    {
        string? value = null;

        var error = Assert.Throws<NanoServiceNotInitialisedException>(
            () => value.AssertReturnGetterValue<string, NanoServiceNotInitialisedException>());

        error.Should().NotBeNull();
    }

    [Test]
    public void AssertReturnGetterValue_WhenValueIsNotNull_ReturnsTheOriginalValue()
    {
        var value = "test-string";
        var response = value.AssertReturnGetterValue<string, NanoServiceNotInitialisedException>();

        response.Should().BeSameAs(value);
    }

    [Test]
    public void AssertReturnGetterValue_WithExceptionPredicate_WhenValueIsNull_ThrowsException()
    {
        string? value = null;

        var error = Assert.Throws<NanoServiceNotInitialisedException>(
            () => value.AssertReturnGetterValue(() => new NanoServiceNotInitialisedException("Custom Message")))!;

        error.Should().NotBeNull();
        error.Message.Should().Be("Custom Message");
    }

    [Test]
    public void AssertReturnGetterValue_WithExceptionPredicate_WhenValueIsNotNull_ReturnsTheOriginalValue()
    {
        var value = "test-string";
        var response = value.AssertReturnGetterValue(() => new NanoServiceNotInitialisedException("Custom Message"));

        response.Should().BeSameAs(value);
    }

    [Test]
    public void AssertReturnGetterValue_WithLogicalAndValuePredicate_WhenValueDoesNotMatchLogicalPredicate_ThrowsException()
    {
        var value = "not-expected-value";

        var error = Assert.Throws<NanoServiceNotInitialisedException>(
            () => value.AssertReturnGetterValue<string, NanoServiceNotInitialisedException>(
                () => value == "expected-value",
                () => value[0].ToString()));

        error.Should().NotBeNull();
    }

    [Test]
    public void AssertReturnGetterValue_WithLogicalAndValuePredicate_WhenValueDoesMatchLogicalPredicate_ReturnsTheValuePredicate()
    {
        var value = "expected-value";
        var response = value.AssertReturnGetterValue<string, NanoServiceNotInitialisedException>(
                () => value == "expected-value",
                () => value[0].ToString());

        response.Should().NotBeSameAs(value);
        response.Should().Be("e");
    }

    [Test]
    public void AssertReturnGetterValue_WithLogicalAndValuePredicateAndExceptionPredicate_WhenValueDoesNotMatchLogicalPredicate_ThrowsException()
    {
        var value = "not-expected-value";

        var error = Assert.Throws<NanoServiceNotInitialisedException>(
            () => value.AssertReturnGetterValue(
                () => value == "expected-value",
                () => value[0].ToString(),
                () => new NanoServiceNotInitialisedException("Custom Message")))!;

        error.Should().NotBeNull();
        error.Message.Should().Be("Custom Message");
    }

    [Test]
    public void AssertReturnGetterValue_WithLogicalAndValuePredicateAndExceptionPredicate_WhenValueDoesMatchLogicalPredicate_ReturnsTheValuePredicate()
    {
        var value = "expected-value";
        var response = value.AssertReturnGetterValue(
            () => value == "expected-value",
            () => value[0].ToString(),
            () => new NanoServiceNotInitialisedException("Custom Message"));

        response.Should().NotBeSameAs(value);
        response.Should().Be("e");
    }
}
