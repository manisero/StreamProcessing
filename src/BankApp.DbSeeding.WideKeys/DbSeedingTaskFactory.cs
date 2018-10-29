using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.DataAccess.WideKeys.Data;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Settings;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Manisero.Navvy.PipelineProcessing;

namespace BankApp.DbSeeding.WideKeys
{
    public class DbSeedingTaskFactory
    {
        private readonly DatabaseManager _databaseManager;
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientSnapshotRepositoryWithSchema _clientSnapshotRepository;
        private readonly DepositSnapshotRepositoryWithSchema _depositSnapshotRepository;
        private readonly LoanSnapshotRepositoryWithSchema _loanSnapshotRepository;

        public DbSeedingTaskFactory(
            DatabaseManager databaseManager,
            DatasetRepository datasetRepository,
            ClientSnapshotRepositoryWithSchema clientSnapshotRepository,
            DepositSnapshotRepositoryWithSchema depositSnapshotRepository,
            LoanSnapshotRepositoryWithSchema loanSnapshotRepository)
        {
            _databaseManager = databaseManager;
            _datasetRepository = datasetRepository;
            _clientSnapshotRepository = clientSnapshotRepository;
            _depositSnapshotRepository = depositSnapshotRepository;
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
                    "DropConstraints",
                    () =>
                    {
                        _loanSnapshotRepository.DropConstraints();
                        _depositSnapshotRepository.DropConstraints();
                        _clientSnapshotRepository.DropConstraints();
                    }),
                new BasicTaskStep(
                    "CreateDatasets",
                    () => state.DatasetIds = CreateDatasets(settings.DatasetsCount)),
                PipelineTaskStep.Builder<DatasetToPopulate>("PopulateDatasets")
                    .WithInput(
                        () => state.DatasetIds.Select(x => new DatasetToPopulate { DatasetId = x }),
                        () => state.DatasetIds.Count)
                    .WithBlock(
                        "CreateClients",
                        x => x.ClientIds = CreateClients(x.DatasetId, settings.ClientsPerDataset))
                    .WithBlock(
                        "CreateDeposits",
                        x => CreateDeposits(x.DatasetId, x.ClientIds, settings.DepositsPerClient))
                    .WithBlock(
                        "CreateLoans",
                        x => CreateLoans(x.DatasetId, x.ClientIds, settings.LoansPerClient))
                    .Build(),
                new BasicTaskStep(
                    "RecreateConstraints_Client",
                    () => _clientSnapshotRepository.RestoreConstraints()),
                new BasicTaskStep(
                    "RecreateConstraints_Deposit",
                    () => _depositSnapshotRepository.RestoreConstraints()),
                new BasicTaskStep(
                    "RecreateConstraints_Loan",
                    () => _loanSnapshotRepository.RestoreConstraints()),
                new BasicTaskStep(
                    "ShrinkAndUpdateStats",
                    () => _databaseManager.ShrinkAndUpdateStats()));
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
            var clients = ClientSnapshot.GetRandom(datasetId, clientsCount).ToArray();
            _clientSnapshotRepository.CreateMany(clients);

            return clients.Select(x => x.ClientId).ToArray();
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
