namespace Splinter.NanoTypes.Default.Domain.Enums;

/// <summary>
/// Depicts the lifetime scope of a collapsed Nano Type instance.
/// </summary>
public enum SuperpositionScope
{
    /// <summary>
    /// The Nano Type is collapsed or initiated per collapse request.
    /// </summary>
    Request = 1,

    /// <summary>
    /// The Nano Type is collapsed once and then the singleton instance is returned per request.
    /// </summary>
    Singleton = 2
}