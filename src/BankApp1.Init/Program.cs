using System;
using System.Diagnostics;
using BankApp1.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.DataSeeding;
using Npgsql;

namespace BankApp1.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var dataSetup = config.GetDataSetup();

            var dbCreated = TryCreateDb(connectionString);

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({dataSetup})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(dataSetup);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
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
    }
}
