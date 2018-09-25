using Manisero.StreamProcessing.DbMigrator;
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
        }

        private static IConfigurationRoot GetConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
