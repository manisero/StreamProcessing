using System;
using Dapper;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataProcessing.Utils.DatabaseAccess
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(
            string connectionString)
        {
            _connectionString = connectionString;
        }

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

        public bool TryRecreate(
            bool force = false,
            Func<string, DbContext> efContextFactory = null,
            Type migrationScriptsAssemblySampleType = null)
        {
            var created = TryRecreateDb(force, efContextFactory);

            if (!created)
            {
                return false;
            }

            if (migrationScriptsAssemblySampleType != null)
            {
                Upgrade(migrationScriptsAssemblySampleType);
            }

            return true;
        }

        private bool TryRecreateDb(
            bool force = false,
            Func<string, DbContext> efContextFactory = null)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString);

            Console.WriteLine($"Creating db '{connectionStringBuilder.Database}'...");
            var isNewDb = EnsureCreated(efContextFactory);

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
            EnsureDeleted();

            Console.WriteLine("Recreating db...");
            EnsureCreated(efContextFactory);

            return true;
        }

        public bool EnsureCreated(
            Func<string, DbContext> efContextFactory = null)
        {
            using (var efContext = efContextFactory != null
                ? efContextFactory(_connectionString)
                : new EmptyEfContext(_connectionString))
            {
                return efContext.Database.EnsureCreated();
            }
        }

        public bool EnsureDeleted()
        {
            using (var efContext = new EmptyEfContext(_connectionString))
            {
                return efContext.Database.EnsureDeleted();
            }
        }

        public void Upgrade(
            Type migrationScriptsAssemblySampleType)
        {
            var upgrader = DeployChanges
                .To.PostgresqlDatabase(_connectionString)
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

        public void ShrinkAndUpdateStats()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute("VACUUM ANALYZE");
            }
        }

        public void CreatePartition<TTable>(
            int forId,
            params string[] pkColumns)
        {
            var tableName = typeof(TTable).Name;
            var pkColumnsString = pkColumns.ToQuotedCommaSeparatedString();

            var sql = $@"
CREATE TABLE ""{tableName}_{forId}""
PARTITION OF ""{tableName}""
(CONSTRAINT ""PK_{tableName}_{forId}"" PRIMARY KEY ({pkColumnsString}))
FOR VALUES IN ({forId})";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void DropPartition<TTable>(
            int forId)
        {
            var tableName = typeof(TTable).Name;
            var sql = $@"DROP TABLE ""{tableName}_{forId}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void CreatePk<TTable>(
            params string[] columns)
        {
            var tableName = typeof(TTable).Name;
            var columnsString = columns.ToQuotedCommaSeparatedString();

            var sql = $@"
ALTER TABLE ""{tableName}""
ADD CONSTRAINT ""PK_{tableName}""
PRIMARY KEY ({columnsString})";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void DropPk<TTable>()
        {
            var tableName = typeof(TTable).Name;

            var sql = $@"
ALTER TABLE ""{tableName}""
DROP CONSTRAINT ""PK_{tableName}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void CreateFk<TFromTable, TToTable>(
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

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void DropFk<TFromTable, TToTable>(
            params string[] columns)
        {
            var fromTableName = typeof(TFromTable).Name;
            var toTableName = typeof(TToTable).Name;
            var columnsNamePart = columns.JoinWithSeparator("_");

            var sql = $@"
ALTER TABLE ""{fromTableName}""
DROP CONSTRAINT ""FK_{fromTableName}_{toTableName}_{columnsNamePart}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
