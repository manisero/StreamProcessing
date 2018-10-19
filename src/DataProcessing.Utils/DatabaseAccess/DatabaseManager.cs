using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataProcessing.Utils.DatabaseAccess
{
    public static class DatabaseManager
    {
        private class EmptyEfContext : DbContext
        {
            private readonly string _connectionString;

            public EmptyEfContext(
                string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override void OnConfiguring(
                DbContextOptionsBuilder optionsBuilder) 
                => optionsBuilder.UseNpgsql(_connectionString);
        }

        public static bool EnsureCreated(
            string connectionString,
            Func<string, DbContext> efContextFactory = null)
        {
            using (var efContext = efContextFactory != null
                ? efContextFactory(connectionString)
                : new EmptyEfContext(connectionString))
            {
                return efContext.Database.EnsureCreated();
            }
        }

        public static bool EnsureDeleted(
            string connectionString)
        {
            using (var efContext = new EmptyEfContext(connectionString))
            {
                return efContext.Database.EnsureDeleted();
            }
        }

        public static bool TryCreate(
            string connectionString,
            Func<string, DbContext> efContextFactory = null)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            Console.WriteLine($"Creating db '{connectionStringBuilder.Database}'...");
            var isNewDb = EnsureCreated(connectionString, efContextFactory);

            if (!isNewDb)
            {
                Console.WriteLine("Db already exists. Recreate? (y - yes; anything else - exit)");
                var answer = Console.ReadLine();

                if (answer != "y")
                {
                    return false;
                }

                Console.WriteLine("Dropping existing db...");
                EnsureDeleted(connectionString);

                Console.WriteLine("Recreating db...");
                EnsureCreated(connectionString, efContextFactory);
            }

            return true;
        }
    }
}
