using System;
using System.Diagnostics;
using BankApp8.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;

namespace BankApp8.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({settings.DataSettings})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(settings.ConnectionString).Seed(settings.DataSettings);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
