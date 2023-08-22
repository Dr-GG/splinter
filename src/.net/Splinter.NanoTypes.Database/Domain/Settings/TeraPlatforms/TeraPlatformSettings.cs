using System;

namespace Splinter.NanoTypes.Database.Domain.Settings.TeraPlatforms;

/// <summary>
/// The startup settings of the ITeraPlatform instance.
/// </summary>
public class TeraPlatformSettings
{
    /// <summary>
    /// The unique ID of the ITeraPlatform instance.
    /// </summary>
    public Guid TeraId { get; set; }
}