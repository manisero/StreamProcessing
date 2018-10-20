using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using BankApp3.Common.DataAccess;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace BankApp3.Main.TotalLoanCalculationTask
{
    public class TotalLoanCalculationTaskFactory
    {
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly TotalLoanCalculationRepository _totalLoanCalculationRepository;

        public TotalLoanCalculationTaskFactory(
            LoanSnapshotRepository loanSnapshotRepository,
            TotalLoanCalculationRepository totalLoanCalculationRepository)
        {
            _loanSnapshotRepository = loanSnapshotRepository;
            _totalLoanCalculationRepository = totalLoanCalculationRepository;
        }

        public TaskDefinition Create(
            short datasetId)
        {
            var totalLoanCalculation = _totalLoanCalculationRepository.Create(
                new TotalLoanCalculation
                {
                    DatasetId = datasetId
                });
            
            var state = new TotalLoanCalculationState();

            return new TaskDefinition(
                $"{nameof(TotalLoanCalculation)}_{totalLoanCalculation.TotalLoanCalculationId}",
                new BasicTaskStep(
                    "LoadData",
                    () =>
                    {
                        state.Loans = _loanSnapshotRepository.GetForDataset(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateTotalLoan",
                    () =>
                    {
                        state.TotalLoan = state.Loans.Sum(x => x.Value);
                    }),
                new BasicTaskStep(
                    "SaveTotalLoan",
                    () =>
                    {
                        _totalLoanCalculationRepository.Update(
                            new TotalLoanCalculation
                            {
                                TotalLoanCalculationId = totalLoanCalculation.TotalLoanCalculationId,
                                DatasetId = totalLoanCalculation.DatasetId,
                                TotalLoan = state.TotalLoan
                            });
                    }));
        }
    }
}
