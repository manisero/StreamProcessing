using System.Linq;
using BankApp8.Common.DataAccess;
using BankApp8.Common.Domain;
using DataProcessing.Utils;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;

namespace BankApp8.Main.ClientLoansCalculationTask
{
    public class ClientLoansCalculationTaskFactory
    {
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly ClientLoansCalculationRepository _clientLoansCalculationRepository;

        public ClientLoansCalculationTaskFactory(
            ClientSnapshotRepository clientSnapshotRepository,
            LoanSnapshotRepository loanSnapshotRepository,
            ClientLoansCalculationRepository clientLoansCalculationRepository)
        {
            _clientSnapshotRepository = clientSnapshotRepository;
            _loanSnapshotRepository = loanSnapshotRepository;
            _clientLoansCalculationRepository = clientLoansCalculationRepository;
        }

        public TaskDefinition Create(
            short datasetId)
        {
            var clientLoansCalculation = _clientLoansCalculationRepository.Create(
                new ClientLoansCalculation
                {
                    DatasetId = datasetId
                });

            var clientsCount = _clientSnapshotRepository.CountInDataset(clientLoansCalculation.DatasetId);
            var batchSize = ClientSnapshot.DefaultReadingBatchSize;
            var batchesCount = clientsCount.CeilingOfDivisionBy(batchSize);
            var clientsReader = _clientSnapshotRepository.GetBatchedReader(clientLoansCalculation.DatasetId, batchSize);

            return new TaskDefinition(
                $"{nameof(ClientLoansCalculation)}_{clientLoansCalculation.ClientLoansCalculationId}",
                PipelineTaskStep
                    .Builder<ClientsToProcess>("Process")
                    .WithInput(
                        clientsReader
                            .Enumerate()
                            .Select(x => new ClientsToProcess
                            {
                                Clients = x.ToDictionary(
                                    c => c.ClientId,
                                    c => new ClientToProcess { Client = c })
                            }),
                        batchesCount)
                    .WithBlock(
                        "LoadLoans",
                        x =>
                        {
                            var loans = _loanSnapshotRepository.GetRange(
                                clientLoansCalculation.DatasetId,
                                x.Clients.First().Key,
                                x.Clients.Last().Key);

                            foreach (var clientIdToLoans in loans)
                            {
                                x.Clients[clientIdToLoans.Key].Loans = clientIdToLoans.Value;
                            }
                        })
                    .WithBlock(
                        "CalculateTotalLoans",
                        x =>
                        {
                            foreach (var client in x.Clients.Values)
                            {
                                client.TotalLoan = client.Loans.Sum(c => c.Value);
                            }
                        })
                    .WithBlock(
                        "SaveClientTotalLoans",
                        x =>
                        {
                            var clientTotalLoans = x.Clients.Values.Select(c => new ClientTotalLoan
                            {
                                ClientLoansCalculationId = clientLoansCalculation.ClientLoansCalculationId,
                                ClientId = c.Client.ClientId,
                                TotalLoan = c.TotalLoan
                            });

                            _clientLoansCalculationRepository.SaveClientTotalLoans(clientTotalLoans);
                        })
                    .Build());
        }
    }
}
