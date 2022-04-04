using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;
using Tenjin.Tests.EntityFramework.Sqlite.Factories;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public class MockTeraDbContextFactory : SqliteEntityFrameworkDbContextFactory<TeraDbContext>
    {
        protected override TeraDbContext Create(DbContextOptions<TeraDbContext> options)
        {
            return new TeraDbContext(options)
            {
                IsSqliteDatabase = true
            };
        }
    }
}
