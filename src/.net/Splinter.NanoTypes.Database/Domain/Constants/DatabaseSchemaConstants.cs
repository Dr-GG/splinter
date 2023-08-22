namespace Splinter.NanoTypes.Database.Domain.Constants;

/// <summary>
/// The collection of database schema constants.
/// </summary>
public static class DatabaseSchemaConstants
{
    /// <summary>
    /// The schema that stores master data such as enumerations, etc.
    /// </summary>
    public const string MasterData = "masterdata";

    /// <summary>
    /// The schema that stores all superposition information such as recollapses.
    /// </summary>
    public const string Superposition = "superposition";

    /// <summary>
    /// The schema that stores all platform information such as ITeraAgentPlatform instances.
    /// </summary>
    public const string Platform = "platform";

    /// <summary>
    /// The schema that stores all Nano Type related information and data.
    /// </summary>
    public const string Nano = "nano";

    /// <summary>
    /// The schema that stores all Tera Type related information and data.
    /// </summary>
    public const string Tera = "tera";
}