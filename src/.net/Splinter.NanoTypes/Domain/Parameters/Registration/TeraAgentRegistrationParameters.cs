using System;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Parameters.Registration;

public record TeraAgentRegistrationParameters : NanoParameters
{
    public Guid? TeraId { get; init; } = null!;
    public SplinterId NanoInstanceId { get; init; } = null!;
    public TeraAgentStatus TeraAgentStatus { get; init; } = TeraAgentStatus.Running;
}