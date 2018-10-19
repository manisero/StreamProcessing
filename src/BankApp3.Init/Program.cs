using System;
using System.Diagnostics;
using BankApp3.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.DataSeeding;

namespace BankApp3.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var dataSetup = config.GetDataSetup();

            var dbCreated = DatabaseManager.TryRecreate(
                connectionString,
                migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({dataSetup})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(dataSetup);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
