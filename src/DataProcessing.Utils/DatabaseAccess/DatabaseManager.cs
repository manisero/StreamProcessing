using System;
using Dapper;
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

        public static void ShrinkAndUpdateStats(
            string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute("VACUUM ANALYZE");
            }
        }

        public static void CreatePk<TTable>(
            string connectionString,
            params string[] columns)
        {
            var tableName = typeof(TTable).Name;
            var columnsString = columns.ToQuotedCommaSeparatedString();

            var sql = $@"
ALTER TABLE ""{tableName}""
ADD CONSTRAINT ""PK_{tableName}""
PRIMARY KEY ({columnsString})";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sql);
            }
        }

        public static void DropPk<TTable>(
            string connectionString)
        {
            var tableName = typeof(TTable).Name;

            var sql = $@"
ALTER TABLE ""{tableName}""
DROP CONSTRAINT ""PK_{tableName}""";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sql);
            }
        }

        public static void CreateFk<TFromTable, TToTable>(
            string connectionString,
            params string[] columns)
        {
            var fromTableName = typeof(TFromTable).Name;
            var toTableName = typeof(TToTable).Name;
            var columnsNamePart = columns.JoinWithSeparator("_");
            var columnsString = columns.ToQuotedCommaSeparatedString();

            var sql = $@"
ALTER TABLE ""{fromTableName}""
ADD CONSTRAINT ""FK_{fromTableName}_{toTableName}_{columnsNamePart}""
FOREIGN KEY ({columnsString})
REFERENCES ""{toTableName}"" ({columnsString})";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sql);
            }
        }

        public static void DropFk<TFromTable, TToTable>(
            string connectionString,
            params string[] columns)
        {
            var fromTableName = typeof(TFromTable).Name;
            var toTableName = typeof(TToTable).Name;
            var columnsNamePart = columns.JoinWithSeparator("_");

            var sql = $@"
ALTER TABLE ""{fromTableName}""
DROP CONSTRAINT ""FK_{fromTableName}_{toTableName}_{columnsNamePart}""";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
