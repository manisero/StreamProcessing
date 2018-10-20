using System.Linq;
using BankApp8.Common.DataAccess;
using BankApp8.Common.Domain;
using DataProcessing.Utils;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;

namespace BankApp8.Main.ClientLoansCalculationTask
{
    public class LoansProcessingTaskFactory
    {
        private readonly ClientSnapshotRepository _clientSnapshotRepository;
        private readonly LoanSnapshotRepository _loanSnapshotRepository;
        private readonly LoansProcessRepository _loansProcessRepository;

        public LoansProcessingTaskFactory(
            ClientSnapshotRepository clientSnapshotRepository,
            LoanSnapshotRepository loanSnapshotRepository,
            LoansProcessRepository loansProcessRepository)
        {
            _clientSnapshotRepository = clientSnapshotRepository;
            _loanSnapshotRepository = loanSnapshotRepository;
            _loansProcessRepository = loansProcessRepository;
        }

        public TaskDefinition Create(
            short datasetId)
        {
            var process = _loansProcessRepository.Create(new LoansProcess { DatasetId = datasetId });

            var clientsCount = _clientSnapshotRepository.CountInDataset(process.DatasetId);
            var batchSize = ClientSnapshot.DefaultReadingBatchSize;
            var batchesCount = clientsCount.CeilingOfDivisionBy(batchSize);
            var clientsReader = _clientSnapshotRepository.GetBatchedReader(process.DatasetId, batchSize);

            return new TaskDefinition(
                $"LoansProcessing_{process.LoansProcessId}",
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
                                process.DatasetId,
                                x.Clients.First().Key,
                                x.Clients.Last().Key);

                            foreach (var clientIdToLoans in loans)
                            {
                                x.Clients[clientIdToLoans.Key].Loans = clientIdToLoans.Value;
                            }
                        })
                    .WithBlock(
                        "SumLoans",
                        x =>
                        {
                            foreach (var client in x.Clients.Values)
                            {
                                client.TotalLoan = client.Loans.Sum(c => c.Value);
                            }
                        })
                    .WithBlock(
                        "SaveClientResults",
                        x => _loansProcessRepository.SaveClientResults(
                            x.Clients.Values.Select(c => new LoansProcessClientResult
                            {
                                LoansProcessId = process.LoansProcessId,
                                ClientId = c.Client.ClientId,
                                TotalLoan = c.TotalLoan
                            })))
                    .Build());
        }
    }
}
