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
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;

        public DbSeeder(
            string connectionString)
        {
            _connectionString = connectionString;
            _datasetRepository = new DatasetRepository(connectionString);
            _clientSnapshotRepository = new ClientSnapshotRepository(connectionString);
            _loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
        }

        public void Seed(
            int datasetsCount,
            int clientsPerDataset,
            int loansPerClient)
        {
            var datasets = CreateDatasets(datasetsCount);

            foreach (var dataset in datasets)
            {
                var clients = CreateClients(dataset.DatasetId, clientsPerDataset);
                CreateLoans(clients, loansPerClient);
            }
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

        private ICollection<ClientSnapshot> CreateClients(
            int datasetId,
            int clientsCount)
        {
            var clients = new List<ClientSnapshot>();

            for (var i = 0; i < clientsCount; i++)
            {
                clients.Add(
                    new ClientSnapshot
                    {
                        ClientId = i + 1,
                        DatasetId = datasetId
                    });
            }

            _clientSnapshotRepository.CreateMany(clients);

            using (var context = new EfContext(_connectionString))
            {
                return context.Set<ClientSnapshot>().ToList();
            }
        }

        private void CreateLoans(
            ICollection<ClientSnapshot> clients,
            int loansPerClient)
        {
            var nextLoanId = 1;
            var loans = new List<LoanSnapshot>();

            foreach (var client in clients)
            {
                for (var i = 0; i < loansPerClient; i++)
                {
                    loans.Add(
                        new LoanSnapshot
                        {
                            LoanId = nextLoanId,
                            ClientSnapshotId = client.ClientSnapshotId,
                            Value = 1m
                        });

                    nextLoanId++;
                }
            }

            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
