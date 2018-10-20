using System;
using System.Linq;
using BankApp.Domain.WideKeys.Tasks;
using BankApp3.Common.DataAccess.Data;
using BankApp3.Common.DataAccess.Tasks;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace BankApp3.Main.MaxLossCalculationTask
{
    public class MaxLossCalculationTaskFactory
    {
        private readonly DepositSnapshotRepository _depositSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly MaxLossCalculationRepository _maxLossCalculationRepository;

        public MaxLossCalculationTaskFactory(
            DepositSnapshotRepository depositSnapshotRepository,
            LoanSnapshotRepository loanSnapshotRepository,
            MaxLossCalculationRepository maxLossCalculationRepository)
        {
            _depositSnapshotRepository = depositSnapshotRepository;
            _loanSnapshotRepository = loanSnapshotRepository;
            _maxLossCalculationRepository = maxLossCalculationRepository;
        }

        public TaskDefinition Create(
            short datasetId)
        {
            var maxLossCalculation = _maxLossCalculationRepository.Create(
                new MaxLossCalculation
                {
                    DatasetId = datasetId
                });
            
            var state = new MaxLossCalculationState();

            return new TaskDefinition(
                $"{nameof(MaxLossCalculation)}_{maxLossCalculation.MaxLossCalculationId}",
                new BasicTaskStep(
                    "LoadData",
                    () =>
                    {
                        state.Deposits = _depositSnapshotRepository.GetForDataset(datasetId);
                        state.Loans = _loanSnapshotRepository.GetForDataset(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateMaxLoss",
                    () =>
                    {
                        var totalLoan = state.Loans.Sum(x => x.Value);
                        var totalDeposit = state.Deposits.Sum(x => x.Value);
                        var maxLoss = totalLoan - totalDeposit;

                        state.MaxLoss = Math.Max(maxLoss, 0m);
                    }),
                new BasicTaskStep(
                    "SaveTotalLoan",
                    () =>
                    {
                        _maxLossCalculationRepository.Update(
                            new MaxLossCalculation
                            {
                                MaxLossCalculationId = maxLossCalculation.MaxLossCalculationId,
                                DatasetId = maxLossCalculation.DatasetId,
                                MaxLoss = state.MaxLoss
                            });
                    }));
        }
    }
}
