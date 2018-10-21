using System;
using System.Diagnostics;
using BankApp.DataAccess.WideKeys;
using BankApp3.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Settings;

namespace BankApp3.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var dbCreationSettings = config.GetDbCreationSettings();
            var dataSettings = config.GetDataSettings();

            var dbCreated = DatabaseManager.TryRecreate(
                connectionString,
                dbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({dataSettings})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(dataSettings);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
