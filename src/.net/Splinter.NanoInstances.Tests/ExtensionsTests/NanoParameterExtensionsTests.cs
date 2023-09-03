
using FluentAssertions;
using NUnit.Framework;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Exceptions.NanoParameters;
using Splinter.NanoTypes.Domain.Parameters;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Registration;

namespace Splinter.NanoInstances.Tests.ExtensionsTests;

public class NanoParameterExtensionsTests
{
    [Test]
    public void Cast_WhenCalledWithValidParameters_ReturnsExpectedResult()
    {
        INanoParameters nanoParameters = new TeraAgentRegistrationParameters();
        var castParameters = nanoParameters.Cast<TeraAgentRegistrationParameters>();

        castParameters.Should().BeSameAs(nanoParameters);
    }

    [Test]
    public void Cast_WhenCalledWithInvalidParameters_ThrowsException()
    {
        var nanoParameters = new TeraAgentRegistrationParameters();
        var error = Assert.Throws<InvalidNanoParametersException>(() => nanoParameters.Cast<NanoInitialisationParameters>())!;

        error.Should().NotBeNull();
        error.Message.Should().Be(
            $"Expected the type {typeof(NanoInitialisationParameters).FullName} but got {typeof(TeraAgentRegistrationParameters).FullName}.");
    }
}
