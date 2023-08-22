using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for Nano Type ID's.
/// </summary>
public static class NanoTypeExtensions
{
    /// <summary>
    /// Converts a Guid instance to a SplinterId instance.
    /// </summary>
    public static SplinterId ToSplinterId(this Guid nanoTypeId)
    {
        return new SplinterId
        {
            Name = nanoTypeId.ToString(),
            Version = "0",
            Guid = nanoTypeId
        };
    }
}