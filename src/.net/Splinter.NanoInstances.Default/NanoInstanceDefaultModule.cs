using System.Diagnostics.CodeAnalysis;
using Autofac;
using Splinter.NanoInstances.Default.Extensions;
using Splinter.NanoTypes.Default.Domain.Settings;
using Tenjin.Autofac.Extensions;

namespace Splinter.NanoInstances.Default;

/// <summary>
/// The Autofac module that registers all dependency injections for the default Splinter services.
/// </summary>
[ExcludeFromCodeCoverage]
public class NanoInstanceDefaultModule : Module
{
    private readonly SplinterDefaultSettings _settings;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoInstanceDefaultModule(SplinterDefaultSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
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