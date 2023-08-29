using System;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Parameters.Registration;

/// <summary>
/// The nano parameters used to register the activation of a Tera Agent.
/// </summary>
public record TeraAgentRegistrationParameters : INanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent.
    /// </summary>
    /// <remarks>
    /// If this property is null, a new ID is generated.
    /// </remarks>
    public Guid? TeraId { get; init; } = null!;

    /// <summary>
    /// The SplinterId of the Tera Agent.
    /// </summary>
    public SplinterId NanoInstanceId { get; init; } = null!;

    /// <summary>
    /// The new status of the Tera Agent.
    /// </summary>
    public TeraAgentStatus TeraAgentStatus { get; init; } = TeraAgentStatus.Running;
}