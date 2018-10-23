using System.Collections.Generic;
using System.Linq;
using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.Domain.SurrogateKeys.Data;
using DataProcessing.Utils.Settings;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;

namespace BankApp.DbSeeding.SurrogateKeys
{
    public class DbSeedingTaskFactory
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly DepositSnapshotRepository _depositSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;

        public DbSeedingTaskFactory(
            string connectionString)
        {
            _datasetRepository = new DatasetRepository(connectionString);
            _clientSnapshotRepository = new ClientSnapshotRepository(connectionString);
            _depositSnapshotRepository = new DepositSnapshotRepository(connectionString);
            _loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
        }

        public TaskDefinition Create(
            DataSettings settings)
        {
            var state = new DbSeedingState();

            return new TaskDefinition(
                $"{nameof(DbSeeding)}",
                new BasicTaskStep(
                    "CreateDatasets",
                    () => state.DatasetIds = CreateDatasets(settings.DatasetsCount)),
                PipelineTaskStep.Builder<DatasetToPopulate>("PopulateDatasets")
                    .WithInput(
                        () => state.DatasetIds.Select(x => new DatasetToPopulate { DatasetId = x }),
                        () => state.DatasetIds.Count)
                    .WithBlock(
                        "CreateClients",
                        x => x.ClientSnapshotIds = CreateClients(x.DatasetId, settings.ClientsPerDataset))
                    .WithBlock(
                        "CreateDeposits",
                        x => CreateDeposits(x.ClientSnapshotIds, settings.DepositsPerClient))
                    .WithBlock(
                        "CreateLoans",
                        x => CreateLoans(x.ClientSnapshotIds, settings.LoansPerClient))
                    .Build());
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
