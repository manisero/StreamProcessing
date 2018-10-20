using System;
using System.Collections.Generic;
using System.Linq;
using BankApp3.Common.DataAccess;
using BankApp3.Common.Domain;
using DataProcessing.Utils;
using DataProcessing.Utils.Settings;

namespace BankApp3.Init.DbSeeding
{
    public class DbSeeder
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;

        public DbSeeder(
            string connectionString)
        {
            _datasetRepository = new DatasetRepository(connectionString);
            _clientSnapshotRepository = new ClientSnapshotRepository(connectionString);
            _loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
        }

        public void Seed(
            DataSettings settings)
        {
            var datasets = CreateDatasets(settings.DatasetsCount);

            foreach (var dataset in datasets)
            {
                var clients = CreateClients(dataset.DatasetId, settings.ClientsPerDataset);
                CreateLoans(dataset.DatasetId, clients, settings.LoansPerClient);
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

            return _datasetRepository.GetAll();
        }

        private ICollection<ClientSnapshot> CreateClients(
            short datasetId,
            int clientsCount)
        {
            var clientIds = new Random()
                .NextUniqueCollection(clientsCount, clientsCount * 2)
                .ToArray();

            var clients = new List<ClientSnapshot>();

            for (var i = 0; i < clientsCount; i++)
            {
                clients.Add(
                    new ClientSnapshot
                    {
                        DatasetId = datasetId,
                        ClientId = clientIds[clients.Count]
                    });
            }

            _clientSnapshotRepository.CreateMany(clients);

            return _clientSnapshotRepository.GetForDataset(datasetId);
        }

        private void CreateLoans(
            short datasetId,
            ICollection<ClientSnapshot> clients,
            int loansPerClient)
        {
            var random = new Random();

            var loansCount = clients.Count * loansPerClient;
            var loanIds = random
                .NextUniqueCollection(loansCount, loansCount * 2)
                .ToArray();

            var loans = new List<LoanSnapshot>();

            foreach (var client in clients)
            {
                for (var i = 0; i < loansPerClient; i++)
                {
                    loans.Add(
                        new LoanSnapshot
                        {
                            DatasetId = datasetId,
                            ClientId = client.ClientId,
                            LoanId = loanIds[loans.Count],
                            Value = random.Next(1000, 1000000)
                        });
                }
            }

            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
