using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext.Models;
using Tenjin.Data.EntityFramework.ValueConverters;

namespace Splinter.NanoInstances.Database.DbContext;

/// <summary>
/// The Splinter DB context.
/// </summary>
public class TeraDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    /// <summary>
    /// Creates a new Instance.
    /// </summary>
    public TeraDbContext(DbContextOptions options) : base(options)
    { }

    /// <summary>
    /// The flag indicating if the DbContext is running in Sqlite.
    /// </summary>
    public bool IsSqliteDatabase { get; set; }

    /// <summary>
    /// The collection of OperatingSystemModel instance.
    /// </summary>
    public DbSet<OperatingSystemModel> OperatingSystems { get; set; } = null!;

    /// <summary>
    /// The collection of TeraPlatformModel instance.
    /// </summary>
    public DbSet<TeraPlatformModel> TeraPlatforms { get; set; } = null!;

    /// <summary>
    /// The collection of NanoTypeModel instance.
    /// </summary>
    public DbSet<NanoTypeModel> NanoTypes { get; set; } = null!;

    /// <summary>
    /// The collection of NanoTypeRecollapseOperationModel instance.
    /// </summary>
    public DbSet<NanoTypeRecollapseOperationModel> NanoTypeRecollapseOperations { get; set; } = null!;

    /// <summary>
    /// The collection of NanoInstanceModel instance.
    /// </summary>
    public DbSet<NanoInstanceModel> NanoInstances { get; set; } = null!;

    /// <summary>
    /// The collection of TeraAgentModel instance.
    /// </summary>
    public DbSet<TeraAgentModel> TeraAgents { get; set; } = null!;

    /// <summary>
    /// The collection of TeraAgentNanoTypeDependencyModel instance.
    /// </summary>
    public DbSet<TeraAgentNanoTypeDependencyModel> TeraAgentNanoTypeDependencies { get; set; } = null!;

    /// <summary>
    /// The collection of TeraMessageModel instance.
    /// </summary>
    public DbSet<TeraMessageModel> TeraMessages { get; set; } = null!;

    /// <summary>
    /// The collection of PendingTeraMessageModel instance.
    /// </summary>
    public DbSet<PendingTeraMessageModel> PendingTeraMessages { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        AddTimestamps(modelBuilder);
    }

    private void AddTimestamps(ModelBuilder modelBuilder)
    {
        if (IsSqliteDatabase)
        {
            AddSqliteTimestamps(modelBuilder);
        }
        else
        {
            AddSqlTimestamps(modelBuilder);
        }
    }

    private static void AddSqlTimestamps(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeraMessageModel>()
            .Property(model => model.ETag)
            .IsRowVersion()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    private static void AddSqliteTimestamps(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeraMessageModel>()
            .Property(model => model.ETag)
            .IsRowVersion()
            .HasColumnType("BLOB")
            .HasConversion(new BinaryTimestampConverter())
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}