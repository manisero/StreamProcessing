using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.SurrogateKeys;
using BankApp1.Common.DataAccess;
using DataProcessing.Utils.Settings;

namespace BankApp1.Init.DbSeeding
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
                var clientSnapshotIds = CreateClients(dataset.DatasetId, settings.ClientsPerDataset);
                CreateLoans(clientSnapshotIds, settings.LoansPerClient);
            }
        }

        private ICollection<Dataset> CreateDatasets(
            int datasetsCount)
        {
            var datasets = Dataset.GetRandom(datasetsCount);
            _datasetRepository.CreateMany(datasets);

            return _datasetRepository.GetAll();
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

        private void CreateLoans(
            ICollection<long> clientSnapshotIds,
            int loansPerClient)
        {
            var loans = LoanSnapshot.GetRandom(
                clientSnapshotIds,
                loansPerClient);

            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
