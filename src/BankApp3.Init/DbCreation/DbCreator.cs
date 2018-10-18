using System;
using DataProcessing.Utils.DatabaseAccess;
using DbUp;
using Npgsql;

namespace BankApp3.Init.DbCreation
{
    public static class DbCreator
    {
        public static bool TryCreate(
            string connectionString)
        {
            var dbCreated = TryCreateDb(connectionString);

            if (!dbCreated)
            {
                return false;
            }

            UpgradeDb(connectionString);

            return true;
        }

        private static bool TryCreateDb(
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

        private static void UpgradeDb(
            string connectionString)
        {
            var upgrader = DeployChanges
                .To.PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(DbCreator).Assembly)
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
