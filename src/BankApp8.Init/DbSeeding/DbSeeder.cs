﻿using BankApp8.Init.DbSeeding.DatasetCreation;
using BankApp8.Init.DbSeeding.DatasetPopulation;
using DataProcessing.Utils.Settings;

namespace BankApp8.Init.DbSeeding
{
    public static class DbSeeder
    {
        public static void Seed(
            string connectionString,
            DataSettings settings)
        {
            var datasets = DatasetsCreator.Create(connectionString, settings.DatasetsCount);

            foreach (var dataset in datasets)
            {
                DatasetPopulator.Populate(dataset.DatasetId, connectionString, settings.ClientsPerDataset, settings.LoansPerClient);
            }
        }
    }
}