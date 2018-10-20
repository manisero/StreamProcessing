using System;
using System.Linq;
using BankApp.Domain.SurrogateKeys.Tasks;
using BankApp1.Common.DataAccess.Data;
using BankApp1.Common.DataAccess.Tasks;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace BankApp1.Main.MaxLossCalculationTask
{
    public class MaxLossCalculationTaskFactory
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly MaxLossCalculationRepository _maxLossCalculationRepository;

        public MaxLossCalculationTaskFactory(
            DatasetRepository datasetRepository,
            MaxLossCalculationRepository maxLossCalculationRepository)
        {
            _datasetRepository = datasetRepository;
            _maxLossCalculationRepository = maxLossCalculationRepository;
        }

        public TaskDefinition Create(
            int datasetId)
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
                        state.Dataset = _datasetRepository.Get(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateMaxLoss",
                    () =>
                    {
                        foreach (var client in state.Dataset.Clients)
                        {
                            var clientLoan = client.Loans.Sum(x => x.Value);
                            var clientDeposit = client.Deposits.Sum(x => x.Value);

                            state.MaxLoss += Math.Max(clientLoan - clientDeposit, 0m);
                        }
                    }),
                new BasicTaskStep(
                    "SaveMaxLoss",
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
