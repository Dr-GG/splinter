using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;

/// <summary>
/// The interface of a service that caches the registration and retrieval of Nano Types and Nano Instances.
/// </summary>
public interface INanoTypeCache
{
    /// <summary>
    /// Registers a Nano Type id alongside a logical id.
    /// </summary>
    void RegisterNanoTypeId(SplinterId nanoType, long id);

    /// <summary>
    /// Registers a Nano Instance id alongside a logical id.
    /// </summary>
    void RegisterNanoInstanceId(SplinterId nanoInstance, long id);

    /// <summary>
    /// Attempts to fetch the logical id registered alongside a Nano Type id.
    /// </summary>
    bool TryGetNanoTypeId(SplinterId nanoType, out long id);

    /// <summary>
    /// Attempts to fetch the logical id registered alongside a Nano Instance id.
    /// </summary>
    bool TryGetNanoInstanceId(SplinterId nanoInstance, out long id);
}