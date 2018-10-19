using System;
using DataProcessing.Utils.DatabaseAccess;
using DbUp;

namespace BankApp3.Init.DbCreation
{
    public static class DbCreator
    {
        public static bool TryCreate(
            string connectionString)
        {
            var dbCreated = DatabaseManager.TryCreate(connectionString);

            if (!dbCreated)
            {
                return false;
            }

            UpgradeDb(connectionString);

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
