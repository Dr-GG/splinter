namespace Splinter.NanoTypes.Default.Domain.Enums;

/// <summary>
/// Depicts the superposition mode or collapse mode.
/// </summary>
public enum SuperpositionMode
{
    /// <summary>
    /// Collapses a Nano Type to a Nano Instance.
    /// </summary>
    Collapse = 1,

    /// <summary>
    /// Recollapses a previously collapsed Nano Type to a Nano Instance.
    /// </summary>
    Recollapse = 2
}