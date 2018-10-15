using Manisero.StreamProcessing.Initializer.DbSeeding.DatasetCreation;
using Manisero.StreamProcessing.Initializer.DbSeeding.DatasetPopulation;

namespace Manisero.StreamProcessing.Initializer.DbSeeding
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
