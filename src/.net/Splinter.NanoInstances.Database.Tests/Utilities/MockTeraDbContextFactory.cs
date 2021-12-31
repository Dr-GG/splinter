using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Tenjin.Tests.Databases.Sqlite.EntityFramework;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public class MockTeraDbContextFactory : SqliteEntityFrameworkDbContextFactory<TeraDbContext>
    {
        protected override TeraDbContext Create(DbContextOptions<TeraDbContext> options)
        {
            return new(options)
            {
                IsSqliteDatabase = true
            };
        }
    }
}
