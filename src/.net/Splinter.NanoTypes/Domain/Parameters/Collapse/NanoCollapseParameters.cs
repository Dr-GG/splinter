using System;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;

namespace Splinter.NanoTypes.Domain.Parameters.Collapse;

/// <summary>
/// The parameters used to collapse a Nano Type to a Nano Instance.
/// </summary>
public record NanoCollapseParameters : NanoParameters
{
    /// <summary>
    /// The flag indicating if the Nano Type should be initialised.
    /// </summary>
    /// <remarks>
    /// The default value is true.
    /// </remarks>
    public bool Initialise { get; init; } = true;

    /// <summary>
    /// The ID to be assigned if the collapsed Nano Type is a Tera Type.
    /// </summary>
    public Guid? TeraAgentId { get; init; }

    /// <summary>
    /// The ID of the Nano Type to be collapsed.
    /// </summary>
    public Guid NanoTypeId { get; init; }

    /// <summary>
    /// The NanoInitialisationParameters to be used when initialising the Nano Type.
    /// </summary>
    public NanoInitialisationParameters? Initialisation { get; init; }
}