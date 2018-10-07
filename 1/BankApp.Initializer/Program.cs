using System;
using System.Diagnostics;
using BankApp.DataAccess;
using BankApp.Initializer.DbSeeding;
using BankApp.Utils;

namespace BankApp.Initializer
{
    class Program
    {
        private const int DatasetsCount = 2;
        private const int ClientsPerDataset = 100;
        private const int LoansPerClient = 2;

        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            Console.WriteLine("Ensuring db created...");
            var efContext = new EfContext(connectionString);
            efContext.Database.EnsureCreated();

            Console.WriteLine("Seeding db...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(DatasetsCount, ClientsPerDataset, LoansPerClient);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
