using System;
using System.Diagnostics;
using BankApp8.Init.DbMigration;
using BankApp8.Init.DbSeeding;
using DataProcessing.Utils;

namespace BankApp8.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

            Migrator.Migrate(connectionString, true, true);

            var sw = Stopwatch.StartNew();
            Seeder.Seed(connectionString, 1000000, 2);
            Console.WriteLine($"Seeding took {sw.Elapsed}.");
        }
    }
}
