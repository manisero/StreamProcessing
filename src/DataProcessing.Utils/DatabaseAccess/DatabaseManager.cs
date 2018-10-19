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
            string connectionString)
        {
            using (var efContext = new EmptyEfContext(connectionString))
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
            string connectionString)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            Console.WriteLine($"Creating db '{connectionStringBuilder.Database}'...");
            var isNewDb = DatabaseManager.EnsureCreated(connectionString);

            if (!isNewDb)
            {
                Console.WriteLine("Db already exists. Recreate? (y - yes; anything else - exit)");
                var answer = Console.ReadLine();

                if (answer != "y")
                {
                    return false;
                }

                Console.WriteLine("Dropping existing db...");
                DatabaseManager.EnsureDeleted(connectionString);

                Console.WriteLine("Recreating db...");
                DatabaseManager.EnsureCreated(connectionString);
            }

            return true;
        }
    }
}
