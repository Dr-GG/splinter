using Splinter.NanoTypes.Database.Domain.Settings.TeraPlatforms;

namespace Splinter.NanoTypes.Database.Domain.Settings.Databases;

/// <summary>
/// The database settings to be used by the database components.
/// </summary>
public class SplinterDatabaseSettings
{
    /// <summary>
    /// The database connection string.
    /// </summary>
    public string DatabaseConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The startup settings of the ITeraPlatform instance.
    /// </summary>
    public TeraPlatformSettings TeraPlatform { get; set; } = new();
}