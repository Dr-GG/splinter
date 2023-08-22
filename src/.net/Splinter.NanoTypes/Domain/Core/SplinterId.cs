using System;

namespace Splinter.NanoTypes.Domain.Core;

/// <summary>
/// The data structure that reflects the ID's for Nano Types and Nano Instances.
/// </summary>
public record SplinterId
{
    /// <summary>
    /// The unique GUID of the SplinterId instance.
    /// </summary>
    public Guid Guid { get; init; }
    
    /// <summary>
    /// The logical name of the SplinterId instance.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The logical version of the SplinterId instance.
    /// </summary>
    public string Version { get; init; } = "1.0.0";
}