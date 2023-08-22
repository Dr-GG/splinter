using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.SplinterIds;

/// <summary>
/// Settings indicating pre-defined ID's to be used for common or universal Splinter agents.
/// </summary>
public class SplinterTeraAgentIdSettings
{
    /// <summary>
    /// The predefined Tera Agent Id, if any, of the ITeraPlatformAgent instance.
    /// </summary>
    public Guid? TeraPlatformId { get; set; }

    /// <summary>
    /// The predefined Tera Agent Id, if any, of the ITeraRegistryAgent instance.
    /// </summary>
    public Guid? TeraRegistryId { get; set; }

    /// <summary>
    /// The predefined Tera Agent Id, if any, of the ITeraMessageAgent instance.
    /// </summary>
    public Guid? TeraMessageId { get; set; }
}