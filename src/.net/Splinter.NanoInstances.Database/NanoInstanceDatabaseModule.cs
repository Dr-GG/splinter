using Autofac;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Splinter.NanoInstances.Database.Extensions;
using Splinter.NanoTypes.Database.Domain.Settings.Databases;
using Tenjin.Autofac.Extensions;

namespace Splinter.NanoInstances.Database;

/// <summary>
/// The Autofac module for the Splinter database module.
/// </summary>
public class NanoInstanceDatabaseModule : Module
{
    private readonly SplinterDatabaseSettings _settings;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoInstanceDatabaseModule(SplinterDatabaseSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        var assembly = typeof(NanoInstanceDatabaseModule).Assembly;

        RegisterDbContext(builder);
        RegisterSettings(builder);

        builder.RegisterMappers(assembly);
        builder.RegisterSplinterDatabaseServices();
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