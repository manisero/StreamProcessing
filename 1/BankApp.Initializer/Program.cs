using System;
using System.Diagnostics;
using BankApp.DataAccess;
using BankApp.Initializer.DbSeeding;
using BankApp.Utils;

namespace BankApp.Initializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            Console.WriteLine("Ensuring db created...");
            var efContext = new EfContext(connectionString);
            efContext.Database.EnsureCreated();

            Console.WriteLine("Seeding db...");
            var seedSw = Stopwatch.StartNew();
            DbSeeder.Seed(connectionString);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
