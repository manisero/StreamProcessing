using System;
using System.Diagnostics;
using Manisero.StreamProcessing.DbMigrator;
using Manisero.StreamProcessing.DbSeeder;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Initializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            Migrator.Migrate(connectionString, true, true);

            var sw = Stopwatch.StartNew();
            Seeder.Seed(connectionString, 1000000, 2);
            Console.WriteLine($"Seeding took {sw.Elapsed}.");
        }
    }
}
