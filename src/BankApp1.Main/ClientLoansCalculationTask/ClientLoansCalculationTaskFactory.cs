using System.Linq;
using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.DataAccess.SurrogateKeys.Tasks;
using BankApp.Domain.SurrogateKeys.Tasks;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace BankApp1.Main.ClientLoansCalculationTask
{
    public class ClientLoansCalculationTaskFactory
    {
        private readonly DatasetRepository _datasetRepository;
        private readonly ClientLoansCalculationRepository _clientLoansCalculationRepository;
        private readonly ClientTotalLoanRepository _clientTotalLoanRepository;

        public ClientLoansCalculationTaskFactory(
            DatasetRepository datasetRepository,
            ClientLoansCalculationRepository clientLoansCalculationRepository,
            ClientTotalLoanRepository clientTotalLoanRepository)
        {
            _datasetRepository = datasetRepository;
            _clientLoansCalculationRepository = clientLoansCalculationRepository;
            _clientTotalLoanRepository = clientTotalLoanRepository;
        }

        public TaskDefinition Create(
            int datasetId)
        {
            var clientLoansCalculation = _clientLoansCalculationRepository.Create(
                new ClientLoansCalculation
                {
                    DatasetId = datasetId
                });

            var state = new ClientLoansCalculationState();

            return new TaskDefinition(
                $"{nameof(ClientLoansCalculation)}_{clientLoansCalculation.ClientLoansCalculationId}",
                new BasicTaskStep(
                    "LoadData",
                    () =>
                    {
                        state.Dataset = _datasetRepository.Get(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateClientLoans",
                    () =>
                    {
                        foreach (var client in state.Dataset.Clients)
                        {
                            state.ClientLoans.Add(
                                client.ClientId,
                                client.Loans.Sum(l => l.Value));
                        }
                    }),
                new BasicTaskStep(
                    "SaveClientTotalLoans",
                    () =>
                    {
                        var clientTotalLoans = state.ClientLoans.Select(
                            x => new ClientTotalLoan
                            {
                                ClientLoansCalculationId = clientLoansCalculation.ClientLoansCalculationId,
                                ClientId = x.Key,
                                TotalLoan = x.Value
                            });

                        _clientTotalLoanRepository.CreateMany(clientTotalLoans);
                    }));
        }
    }
}
