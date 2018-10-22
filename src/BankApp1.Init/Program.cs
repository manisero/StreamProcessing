using System;
using System.Diagnostics;
using BankApp.DataAccess.SurrogateKeys;
using BankApp1.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;

namespace BankApp1.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

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
