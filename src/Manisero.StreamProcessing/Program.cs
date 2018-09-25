using Manisero.StreamProcessing.DbMigrator;
using Manisero.StreamProcessing.DbSeeder;
using Microsoft.Extensions.Configuration;

namespace Manisero.StreamProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfig();
            var connectionString = config.GetConnectionString("Default");

            Migrator.Migrate(connectionString, true, true);
            Seeder.Seed(connectionString, 1000, 2);
        }

        private static IConfigurationRoot GetConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
