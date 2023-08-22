namespace Splinter.NanoTypes.Domain.Enums;

/// <summary>
/// Depicts an error code attached to a TeraMessage instance.
/// </summary>
public enum TeraMessageErrorCode
{
    /// <summary>
    /// An unknown error.
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// The maximum dequeue retry count has been reached on the TeraMessage.
    /// </summary>
    MaximumDequeueRetryCountReached = 2,

    /// <summary>
    /// The TeraMessage instance has been disposed of.
    /// </summary>
    Disposed = 3
}