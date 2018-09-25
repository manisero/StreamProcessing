using Manisero.StreamProcessing.DbSeeder.DatasetCreation;
using Manisero.StreamProcessing.DbSeeder.DatasetPopulation;

namespace Manisero.StreamProcessing.DbSeeder
{
    public static class Seeder
    {
        public static void Seed(
            string connectionString,
            int clientsCount,
            int loansPerClient)
        {
            var datasets = DatasetsCreator.Create(connectionString);

            foreach (var datasetId in datasets.Keys)
            {
                DatasetPopulator.Populate(datasetId, connectionString, clientsCount, loansPerClient);
            }
        }
    }
}
