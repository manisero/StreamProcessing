using System;
using DbUp;

namespace BankApp8.Init.DbMigration
{
    public static class Migrator
    {
        public static void Migrate(
            string connectionString,
            bool createDatabase = false,
            bool logToConsole = false)
        {
            if (createDatabase)
            {
                EnsureDatabase.For.PostgresqlDatabase(connectionString);
            }

            var engineBuilder = DeployChanges
                .To.PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(Migrator).Assembly)
                .WithTransaction();

            if (logToConsole)
            {
                engineBuilder = engineBuilder
                    .LogToConsole()
                    .LogScriptOutput();
            }

            var upgrader = engineBuilder.Build();

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
