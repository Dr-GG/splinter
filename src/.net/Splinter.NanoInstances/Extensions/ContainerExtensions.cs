using Autofac;

namespace Splinter.NanoInstances.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterSettings(this ContainerBuilder container, object settings)
        {
            container.RegisterInstance(settings).AsSelf().SingleInstance();
        }
    }
}
