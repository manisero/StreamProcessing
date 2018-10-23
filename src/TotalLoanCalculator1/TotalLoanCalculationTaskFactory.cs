using System.Linq;
using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.DataAccess.SurrogateKeys.Tasks;
using BankApp.Domain.SurrogateKeys.Tasks;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace TotalLoanCalculator1
{
    public class TotalLoanCalculationTaskFactory
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly TotalLoanCalculationRepository _totalLoanCalculationRepository;

        public TotalLoanCalculationTaskFactory(
            DatasetRepository datasetRepository,
            TotalLoanCalculationRepository totalLoanCalculationRepository)
        {
            _datasetRepository = datasetRepository;
            _totalLoanCalculationRepository = totalLoanCalculationRepository;
        }

        public TaskDefinition Create(
            int datasetId)
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
                        state.Dataset = _datasetRepository.Get(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateTotalLoan",
                    () =>
                    {
                        foreach (var client in state.Dataset.Clients)
                        {
                            state.TotalLoan += client.Loans.Sum(x => x.Value);
                        }
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
