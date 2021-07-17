using Splinter.NanoTypes.Database.Domain.Settings.TeraPlatforms;

namespace Splinter.NanoTypes.Database.Domain.Settings.Databases
{
    public class SplinterDatabaseSettings
    {
        public string DatabaseConnectionString { get; set; } = string.Empty;
        public TeraPlatformSettings TeraPlatform { get; set; } = null!;
    }
}
