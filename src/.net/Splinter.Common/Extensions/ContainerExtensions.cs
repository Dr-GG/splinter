using System.Linq;
using System.Reflection;
using Autofac;
using Splinter.Common.Mappers;

namespace Splinter.Common.Extensions
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
    }
}
