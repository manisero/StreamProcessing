using System;
using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp8.Common.DataAccess;

namespace BankApp8.Init.DbSeeding.DatasetCreation
{
    public static class DatasetsCreator
    {
        public static ICollection<Dataset> Create(
            string connectionString,
            int datasetsCount)
        {
            var datasets = new List<Dataset>();

            for (short datasetId = 1; datasetId <= datasetsCount; datasetId++)
            {
                datasets.Add(
                    new Dataset
                    {
                        DatasetId = datasetId,
                        Date = new DateTime(2018, 1, 1).AddMonths(datasetId - 1)
                    });
            }

            var datasetRepository = new DatasetRepository(connectionString);
            datasetRepository.CreateMany(datasets);

            return datasetRepository.GetAll();
        }
    }
}
