using System.Linq;
using BankApp3.Common.DataAccess;
using BankApp3.Common.Domain;
using DataProcessing.Utils;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;

namespace BankApp3.Main.ClientLoansCalculationTask
{
    public class ClientLoansCalculationTaskFactory
    {
        private readonly ClientLoansCalculationRepository _clientLoansCalculationRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly ClientTotalLoanRepository _clientTotalLoanRepository;

        public ClientLoansCalculationTaskFactory(
            ClientLoansCalculationRepository clientLoansCalculationRepository,
            LoanSnapshotRepository loanSnapshotRepository,
            ClientTotalLoanRepository clientTotalLoanRepository)
        {
            _clientLoansCalculationRepository = clientLoansCalculationRepository;
            _loanSnapshotRepository = loanSnapshotRepository;
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
