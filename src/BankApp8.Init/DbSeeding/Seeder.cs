using BankApp8.Init.DbSeeding.DatasetCreation;
using BankApp8.Init.DbSeeding.DatasetPopulation;

namespace BankApp8.Init.DbSeeding
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
