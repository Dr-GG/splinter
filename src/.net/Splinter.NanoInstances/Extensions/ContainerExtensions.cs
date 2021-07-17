using System.Linq;
using System.Reflection;
using Autofac;
using Splinter.NanoInstances.Mappers;
using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterUnaryMappers(this ContainerBuilder container, Assembly assembly)
        {
            container
                .RegisterAssemblyTypes(assembly)
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType
                              && i.GetGenericTypeDefinition() == typeof(IUnaryMapper<,>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        public static void RegisterBinaryMapper(this ContainerBuilder container)
        {
            container
                .RegisterGeneric(typeof(BinaryMapper<,>))
                .As(typeof(IBinaryMapper<,>))
                .InstancePerLifetimeScope();
        }

        public static void RegisterMappers(this ContainerBuilder container, Assembly assembly)
        {
            container.RegisterUnaryMappers(assembly);
            container.RegisterBinaryMapper();
        }

        public static void RegisterSettings(this ContainerBuilder container, object settings)
        {
            container.RegisterInstance(settings).AsSelf().SingleInstance();
        }
    }
}
