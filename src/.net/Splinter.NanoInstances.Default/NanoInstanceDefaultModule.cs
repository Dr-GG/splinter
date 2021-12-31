using Autofac;
using Splinter.NanoInstances.Default.Extensions;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Default.Domain.Settings;
using Tenjin.Autofac.Extensions;

namespace Splinter.NanoInstances.Default
{
    public class NanoInstanceDefaultModule : Module
    {
        private readonly SplinterDefaultSettings _settings;

        public NanoInstanceDefaultModule(SplinterDefaultSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(NanoInstanceDefaultModule).Assembly;

            RegisterSettings(builder);

            builder.RegisterMappers(assembly);
            builder.RegisterDefaultServices();
        }

        private void RegisterSettings(ContainerBuilder container)
        {
            container.RegisterSettings(_settings);
            container.RegisterSettings(_settings.Messaging);
            container.RegisterSettings(_settings.NanoTypeCache);
            container.RegisterSettings(_settings.TeraAgentCache);
            container.RegisterSettings(_settings.SplinterTeraAgentIds);
            container.RegisterSettings(_settings.Superposition);
        }
    }
}
