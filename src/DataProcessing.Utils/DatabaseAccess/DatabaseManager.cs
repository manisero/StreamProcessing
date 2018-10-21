using System;
using DbUp;
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

        public static bool TryRecreate(
            string connectionString,
            bool force = false,
            Func<string, DbContext> efContextFactory = null,
            Type migrationScriptsAssemblySampleType = null)
        {
            var created = TryRecreateDb(connectionString, force, efContextFactory);

            if (!created)
            {
                return false;
            }

            if (migrationScriptsAssemblySampleType != null)
            {
                Upgrade(connectionString, migrationScriptsAssemblySampleType);
            }

            return true;
        }

        private static bool TryRecreateDb(
            string connectionString,
            bool force = false,
            Func<string, DbContext> efContextFactory = null)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            Console.WriteLine($"Creating db '{connectionStringBuilder.Database}'...");
            var isNewDb = EnsureCreated(connectionString, efContextFactory);

            if (isNewDb)
            {
                return true;
            }

            if (!force)
            {
                Console.WriteLine("Db already exists. Recreate? (y - yes; anything else - exit)");
                var answer = Console.ReadLine();

                if (answer != "y")
                {
                    return false;
                }
            }
            
            Console.WriteLine("Dropping existing db...");
            EnsureDeleted(connectionString);

            Console.WriteLine("Recreating db...");
            EnsureCreated(connectionString, efContextFactory);

            return true;
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

        public static void Upgrade(
            string connectionString,
            Type migrationScriptsAssemblySampleType)
        {
            var upgrader = DeployChanges
                .To.PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(migrationScriptsAssemblySampleType.Assembly)
                .WithTransaction()
                .LogToConsole()
                .LogScriptOutput()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new InvalidOperationException(
                    "Error while updating database schema. See inner exception for details.",
                    result.Error);
            }
        }
    }
}
