using Microsoft.Extensions.Configuration;

namespace Splinter.Bootstrap.Extensions
{
    public static class ConfigurationExtensions
    {
        public static TSettings BindSettings<TSettings>(this IConfiguration configuration, string key)
            where TSettings : class, new()
        {
            var settings = new TSettings();

            configuration.Bind(key, settings);

            return settings;
        }
    }
}
