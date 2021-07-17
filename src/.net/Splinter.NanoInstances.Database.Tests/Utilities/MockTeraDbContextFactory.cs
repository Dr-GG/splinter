using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Splinter.NanoInstances.Database.DbContext;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public class MockTeraDbContextFactory : IAsyncDisposable
    {
        private SqliteConnection? _connection;
        private TeraDbContext? _context;

        public TeraDbContext Context
        {
            get
            {
                OpenConnection();

                return _context ??= NewDbContext();
            }
        }

        public ValueTask DisposeAsync()
        {
            _connection?.Dispose();

            return ValueTask.CompletedTask;
        }

        private void OpenConnection()
        {
            if (_connection != null)
            {
                return;
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _connection.EnableExtensions();

            using var createContext = NewDbContext();

            if (!createContext.Database.EnsureCreated())
            {
                throw new InvalidOperationException("SQLite memory provider failed");
            }
        }

        private DbContextOptions<TeraDbContext> GetOptions()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("No open SQLite connection");
            }

            return new DbContextOptionsBuilder<TeraDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(_connection)
                .Options;
        }

        private TeraDbContext NewDbContext()
        {
            return new(GetOptions())
            {
                IsSqliteDatabase = true
            };
        }
    }
}
