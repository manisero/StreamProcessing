using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.Domain.SurrogateKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Settings;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;

namespace BankApp.DbSeeding.SurrogateKeys
{
    public class DbSeedingTaskFactory
    {
        private readonly DatabaseManager _databaseManager;
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;

        public DbSeedingTaskFactory(
            DatabaseManager databaseManager,
            DatasetRepository datasetRepository,
            ClientSnapshotRepository clientSnapshotRepository,
            LoanSnapshotRepository loanSnapshotRepository)
        {
            _databaseManager = databaseManager;
            _datasetRepository = datasetRepository;
            _clientSnapshotRepository = clientSnapshotRepository;
            _loanSnapshotRepository = loanSnapshotRepository;
        }

        public TaskDefinition Create(
            DataSettings settings)
        {
            var state = new DbSeedingState();

            return new TaskDefinition(
                $"{nameof(DbSeeding)}",
                new BasicTaskStep(
                    "PrintSettings",
                    () => Console.WriteLine(settings)),
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
                        "CreateLoans",
                        x => CreateLoans(x.ClientSnapshotIds, settings.LoansPerClient))
                    .Build(),
                new BasicTaskStep(
                    "ShrinkAndUpdateStats",
                    () => _databaseManager.ShrinkAndUpdateStats()));
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

        private void CreateLoans(
            ICollection<long> clientSnapshotIds,
            int loansPerClient)
        {
            var loans = LoanSnapshot.GetRandom(clientSnapshotIds, loansPerClient);
            _loanSnapshotRepository.CreateMany(loans);
        }
    }
}
