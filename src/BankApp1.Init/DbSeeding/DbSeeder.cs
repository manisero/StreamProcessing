using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.SurrogateKeys.Data;
using BankApp1.Common.DataAccess.Data;
using DataProcessing.Utils.Settings;

namespace BankApp1.Init.DbSeeding
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
                var clientSnapshotIds = CreateClients(datasetId, settings.ClientsPerDataset);
                CreateDeposits(clientSnapshotIds, settings.DepositsPerClient);
                CreateLoans(clientSnapshotIds, settings.LoansPerClient);
            }
        }

        private ICollection<int> CreateDatasets(
            int datasetsCount)
        {
            var datasets = Dataset.GetRandom(datasetsCount);
            _datasetRepository.CreateMany(datasets);

            return _datasetRepository
                .GetAll()
                .Select(x => x.DatasetId)
                .ToArray();
        }

        private ICollection<long> CreateClients(
            int datasetId,
            int clientsCount)
        {
            var clients = ClientSnapshot.GetRandom(datasetId, clientsCount);
            _clientSnapshotRepository.CreateMany(clients);

            return _clientSnapshotRepository
                .GetForDataset(datasetId)
                .Select(x => x.ClientSnapshotId)
                .ToArray();
        }

        private void CreateDeposits(
            ICollection<long> clientSnapshotIds,
            int depositsPerClient)
        {
            var deposits = DepositSnapshot.GetRandom(clientSnapshotIds, depositsPerClient);
            _depositSnapshotRepository.CreateMany(deposits);
        }

        private void CreateLoans(
            ICollection<long> clientSnapshotIds,
            int loansPerClient)
        {
            var loans = LoanSnapshot.GetRandom(clientSnapshotIds, loansPerClient);
            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
