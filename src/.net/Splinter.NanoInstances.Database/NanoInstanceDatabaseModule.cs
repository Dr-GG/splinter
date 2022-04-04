using Autofac;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoTypes.Database.Domain.Settings.Databases;
using Tenjin.Autofac.Extensions;

namespace Splinter.NanoInstances.Database
{
    public class NanoInstanceDatabaseModule : Module
    {
        private readonly SplinterDatabaseSettings _settings;

        public NanoInstanceDatabaseModule(SplinterDatabaseSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(NanoInstanceDatabaseModule).Assembly;

            RegisterDbContext(builder);
            RegisterSettings(builder);

            builder.RegisterMappers(assembly);
            builder.RegisterDatabaseServices();
        }

        private void RegisterSettings(ContainerBuilder builder)
        {
            builder.RegisterSingleton(_settings);
            builder.RegisterSingleton(_settings.TeraPlatform);
        }

        private void RegisterDbContext(ContainerBuilder builder)
        {
            builder
                .Register(_ =>
                {
                    var options = new DbContextOptionsBuilder()
                        .UseSqlServer(_settings.DatabaseConnectionString)
                        .Options;

                    return new TeraDbContext(options);
                }).AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}