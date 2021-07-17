using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext.Models;
using Splinter.NanoInstances.Database.DbContext.ValueConverters;

namespace Splinter.NanoInstances.Database.DbContext
{
    public class TeraDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public TeraDbContext(DbContextOptions options) : base(options)
        { }

        public bool IsSqliteDatabase { get; set; }

        public DbSet<OperatingSystemModel> OperatingSystems { get; set; } = null!;
        public DbSet<TeraPlatformModel> TeraPlatforms { get; set; } = null!;
        public DbSet<NanoTypeModel> NanoTypes { get; set; } = null!;
        public DbSet<NanoTypeRecollapseOperationModel> NanoTypeRecollapseOperations { get; set; } = null!;
        public DbSet<NanoInstanceModel> NanoInstances { get; set; } = null!;
        public DbSet<TeraAgentModel> TeraAgents { get; set; } = null!;
        public DbSet<TeraAgentNanoTypeDependencyModel> TeraAgentNanoTypeDependencies { get; set; } = null!;
        public DbSet<TeraMessageModel> TeraMessages { get; set; } = null!;
        public DbSet<PendingTeraMessageModel> PendingTeraMessages { get; set; } = null!;

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
                .HasConversion(new SqliteTimestampConverter())
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
