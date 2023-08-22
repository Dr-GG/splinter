namespace Splinter.NanoTypes.Domain.Enums;

/// <summary>
/// The status of a TeraMessage instance.
/// </summary>
public enum TeraMessageStatus
{
    /// <summary>
    /// The TeraMessage is pending processing.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// The TeraMessage has been dequeued and awaiting processing.
    /// </summary>
    Dequeued = 2,

    /// <summary>
    /// The TeraMessage has been cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// The TeraMessage was successfully processed.
    /// </summary>
    Completed = 4,

    /// <summary>
    /// The TeraMessage failed.
    /// </summary>
    Failed = 5
}