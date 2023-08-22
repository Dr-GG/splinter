using Autofac;
using Splinter.NanoInstances.Default.Interfaces.Superposition;
using Splinter.NanoInstances.Default.Services.Messaging;
using Splinter.NanoInstances.Default.Services.NanoTypes;
using Splinter.NanoInstances.Default.Services.OperatingSystems;
using Splinter.NanoInstances.Default.Services.Superposition;
using Splinter.NanoInstances.Default.Services.TeraAgents;
using Splinter.NanoInstances.Interfaces.WaveFunctions;
using Splinter.NanoInstances.Services.Builders;
using Splinter.NanoTypes.Default.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Superposition;

namespace Splinter.NanoInstances.Default.Extensions;

/// <summary>
/// The collection of Autofac container extensions.
/// </summary>
public static class ContainerExtensions
{
    /// <summary>
    /// Registers all default Splinter services.
    /// </summary>
    public static void RegisterDefaultServices(this ContainerBuilder container)
    {
        container
            .RegisterType<JsonSuperpositionMappingResolver>()
            .As<ISuperpositionMappingResolver>()
            .InstancePerLifetimeScope();

        container
            .RegisterType<SuperpositionMappingRegistry>()
            .As<ISuperpositionMappingRegistry>()
            .SingleInstance();

        container
            .RegisterType<SuperpositionSingletonRegistry>()
            .As<ISuperpositionSingletonRegistry>()
            .SingleInstance();

        container
            .RegisterType<ActivatorNanoWaveFunctionBuilder>()
            .As<INanoWaveFunctionBuilder>()
            .InstancePerLifetimeScope();

        container
            .RegisterType<OperatingSystemInformationProvider>()
            .As<IOperatingSystemInformationProvider>()
            .InstancePerLifetimeScope();

        container
            .RegisterType<NanoTypeCache>()
            .As<INanoTypeCache>()
            .SingleInstance();

        container
            .RegisterType<TeraAgentCache>()
            .As<ITeraAgentCache>()
            .SingleInstance();

        container
            .RegisterType<NanoTable>()
            .As<INanoTable>()
            .InstancePerLifetimeScope();

        container
            .RegisterType<TeraMessageQueue>()
            .As<ITeraMessageQueue>()
            .InstancePerLifetimeScope();

        container
            .RegisterType<RecollapseNanoTypeService>()
            .As<IRecollapseNanoTypeService>()
            .InstancePerLifetimeScope();
    }
}