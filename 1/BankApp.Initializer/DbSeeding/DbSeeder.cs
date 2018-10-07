using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.DataAccess;
using BankApp.Domain;

namespace BankApp.Initializer.DbSeeding
{
    public class DbSeeder
    {
        private readonly string _connectionString;
        private readonly DatasetRepository _datasetRepository;

        public DbSeeder(
            string connectionString)
        {
            _connectionString = connectionString;
            _datasetRepository = new DatasetRepository(connectionString);
        }

        public void Seed(
            int datasetsCount,
            int clientsPerDataset,
            int loansPerClient)
        {
            var datasets = CreateDatasets(datasetsCount);
        }

        private ICollection<Dataset> CreateDatasets(
            int datasetsCount)
        {
            var datasets = new List<Dataset>();

            for (var i = 0; i < datasetsCount; i++)
            {
                datasets.Add(
                    new Dataset
                    {
                        Date = new DateTime(2018, 1, 1).AddMonths(i)
                    });
            }

            _datasetRepository.CreateMany(datasets);

            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().ToList();
            }
        }
    }
}
