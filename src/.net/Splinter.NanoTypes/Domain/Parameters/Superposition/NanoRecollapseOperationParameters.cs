using System;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

/// <summary>
/// The nano parameters used to sync the state of a recollapse operation.
/// </summary>
public record NanoRecollapseOperationParameters : NanoParameters
{
    /// <summary>
    /// The flag indicating if the recollapse was successful or not.
    /// </summary>
    public bool IsSuccessful { get; init; }

    /// <summary>
    /// The ID of the nano recollapse operation.
    /// </summary>
    public Guid NanoRecollapseOperationId { get; init; }

    /// <summary>
    /// The ID of the Tera Agent that the recollapse operation was applied to.
    /// </summary>
    public Guid TeraId { get; init; }
}