using Autofac;
using Splinter.NanoInstances.Database.Services.Messaging;
using Splinter.NanoInstances.Database.Services.NanoTypes;
using Splinter.NanoInstances.Database.Services.OperatingSystems;
using Splinter.NanoInstances.Database.Services.Registration;
using Splinter.NanoInstances.Database.Services.Superposition;
using Splinter.NanoInstances.Database.Services.TeraAgents;
using Splinter.NanoInstances.Database.Services.TeraPlatforms;
using Splinter.NanoTypes.Database.Interfaces.Services.Messaging;
using Splinter.NanoTypes.Database.Interfaces.Services.OperatingSystems;
using Splinter.NanoTypes.Database.Interfaces.Services.Registration;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Default.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraPlatforms;

namespace Splinter.NanoInstances.Database.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterDatabaseServices(this ContainerBuilder container)
        {
            container
                .RegisterType<OperatingSystemManager>()
                .As<IOperatingSystemManager>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraPlatformManager>()
                .As<ITeraPlatformManager>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<NanoTypeManager>()
                .As<INanoTypeManager>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraAgentManager>()
                .As<ITeraAgentManager>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraAgentRegistrationService>()
                .As<ITeraAgentRegistrationService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraMessageRelayService>()
                .As<ITeraMessageRelayService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraMessageDequeueService>()
                .As<ITeraMessageDequeueService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraMessageDisposeService>()
                .As<ITeraMessageDisposeService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraMessageSyncService>()
                .As<ITeraMessageSyncService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<TeraAgentNanoTypeDependencyService>()
                .As<ITeraAgentNanoTypeDependencyService>()
                .InstancePerLifetimeScope();

            container
                .RegisterType<NanoTypeRecollapseOperationService>()
                .As<INanoTypeRecollapseOperationService>()
                .InstancePerLifetimeScope();
        }
    }
}
