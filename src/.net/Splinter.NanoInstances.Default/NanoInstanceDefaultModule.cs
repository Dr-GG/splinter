using Autofac;
using Splinter.NanoInstances.Default.Extensions;
using Splinter.NanoTypes.Default.Domain.Settings;
using Tenjin.Autofac.Extensions;

namespace Splinter.NanoInstances.Default;

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
        container.RegisterSingleton(_settings);
        container.RegisterSingleton(_settings.Messaging);
        container.RegisterSingleton(_settings.NanoTypeCache);
        container.RegisterSingleton(_settings.TeraAgentCache);
        container.RegisterSingleton(_settings.SplinterTeraAgentIds);
        container.RegisterSingleton(_settings.Superposition);
    }
}