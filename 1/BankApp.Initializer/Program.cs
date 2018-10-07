using System;
using System.Diagnostics;
using BankApp.DataAccess;
using BankApp.Initializer.DbSeeding;
using BankApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Initializer
{
    class Program
    {
        private const int DatasetsCount = 2;
        private const int ClientsPerDataset = 100000;
        private const int LoansPerClient = 2;

        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            var dbCreated = TryCreateDb(connectionString);

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine("Seeding db...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(DatasetsCount, ClientsPerDataset, LoansPerClient);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }

        private static bool TryCreateDb(
            string connectionString)
        {
            using (var efContext = new EfContext(connectionString))
            {
                var connection = efContext.Database.GetDbConnection();

                Console.WriteLine($"Creating db '{connection.Database}'...");
                var isNewDb = efContext.Database.EnsureCreated();

                if (!isNewDb)
                {
                    Console.WriteLine("Db already exists. Recreate? (y - yes; anything else - exit)");
                    var answer = Console.ReadLine();

                    if (answer == "y")
                    {
                        Console.WriteLine("Dropping existing db...");
                        efContext.Database.EnsureDeleted();
                        Console.WriteLine("Recreating db...");
                        efContext.Database.EnsureCreated();
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
