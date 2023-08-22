using System;

namespace Splinter.NanoTypes.Default.Domain.Messaging.Superposition;

/// <summary>
/// The TeraMessage.Message data structure that holds recollapse information.
/// </summary>
public record RecollapseTeraMessageData
{
    /// <summary>
    /// The ID of the Nano Type recollapse operation to be used for synchronising recollapse progress.
    /// </summary>
    public Guid NanoTypeRecollapseOperationId { get; init; }

    /// <summary>
    /// The ID of the Nano Type to be recollapsed.
    /// </summary>
    public Guid NanoTypeId { get; init; }
}