namespace Splinter.NanoTypes.Domain.Enums;

public enum SplinterEnvironmentStatus
{
    /// <summary>
    /// The Splinter environment is uninitialised.
    /// </summary>
    Uninitialised = 1,

    /// <summary>
    /// The Splinter environment is busy initialising.
    /// </summary>
    Initialising = 2,

    /// <summary>
    /// The Splinter environment has been fully initialised and is running.
    /// </summary>
    Initialised = 3,

    /// <summary>
    /// The Splinter environment is busy shutting down and disposing.
    /// </summary>
    Disposing = 4,

    /// <summary>
    /// The Splinter environment is now disposed and shut down.
    /// </summary>
    Disposed = 5
}