using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp3.Common.DataAccess;
using DataProcessing.Utils.Settings;

namespace BankApp3.Init.DbSeeding
{
    public class DbSeeder
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly DepositSnapshotRepository _depositSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;

        public DbSeeder(
            string connectionString)
        {
            _datasetRepository = new DatasetRepository(connectionString);
            _clientSnapshotRepository = new ClientSnapshotRepository(connectionString);
            _depositSnapshotRepository = new DepositSnapshotRepository(connectionString);
            _loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
        }

        public void Seed(
            DataSettings settings)
        {
            var datasetIds = CreateDatasets(settings.DatasetsCount);

            foreach (var datasetId in datasetIds)
            {
                var clientIds = CreateClients(datasetId, settings.ClientsPerDataset);
                CreateDeposits(datasetId, clientIds, settings.DepositsPerClient);
                CreateLoans(datasetId, clientIds, settings.LoansPerClient);
            }
        }

        private ICollection<short> CreateDatasets(
            int datasetsCount)
        {
            var datasets = Dataset.GetRandom(datasetsCount);
            _datasetRepository.CreateMany(datasets);

            return _datasetRepository
                .GetAll()
                .Select(x => x.DatasetId)
                .ToArray();
        }

        private ICollection<int> CreateClients(
            short datasetId,
            int clientsCount)
        {
            var clients = ClientSnapshot.GetRandom(datasetId, clientsCount);
            _clientSnapshotRepository.CreateMany(clients);

            return _clientSnapshotRepository
                .GetForDataset(datasetId)
                .Select(x => x.ClientId)
                .ToArray();
        }

        private void CreateDeposits(
            short datasetId,
            ICollection<int> clientIds,
            int depositsPerClient)
        {
            var deposits = DepositSnapshot.GetRandom(datasetId, clientIds, depositsPerClient);
            _depositSnapshotRepository.CreateMany(deposits);
        }

        private void CreateLoans(
            short datasetId,
            ICollection<int> clientIds,
            int loansPerClient)
        {
            var loans = LoanSnapshot.GetRandom(datasetId, clientIds, loansPerClient);
            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
