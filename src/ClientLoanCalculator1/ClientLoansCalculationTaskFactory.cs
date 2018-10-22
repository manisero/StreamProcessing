using System.Linq;
using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace ClientLoanCalculator1
{
    public class ClientLoansCalculationTaskFactory
    {
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly ClientLoansCalculationRepository _clientLoansCalculationRepository;
        private readonly ClientTotalLoanRepository _clientTotalLoanRepository;

        public ClientLoansCalculationTaskFactory(
            LoanSnapshotRepository loanSnapshotRepository,
            ClientLoansCalculationRepository clientLoansCalculationRepository,
            ClientTotalLoanRepository clientTotalLoanRepository)
        {
            _loanSnapshotRepository = loanSnapshotRepository;
            _clientLoansCalculationRepository = clientLoansCalculationRepository;
            _clientTotalLoanRepository = clientTotalLoanRepository;
        }

        public TaskDefinition Create(
            short datasetId)
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
                        state.Loans = _loanSnapshotRepository.GetForDataset(datasetId);
                    }),
                new BasicTaskStep(
                    "CalculateTotalLoans",
                    () =>
                    {
                        foreach (var loan in state.Loans)
                        {
                            state.ClientLoans[loan.ClientId] = loan.Value + state.ClientLoans.GetValueOrDefault(loan.ClientId);
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
