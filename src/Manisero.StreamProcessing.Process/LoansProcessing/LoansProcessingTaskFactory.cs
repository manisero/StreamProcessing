using System.Linq;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Process.DataAccess;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.LoansProcessing
{
    public interface ILoansProcessingTaskFactory
    {
        TaskDefinition Create(
            LoansProcess process);
    }

    public class LoansProcessingTaskFactory : ILoansProcessingTaskFactory
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoansProcessRepository _loansProcessRepository;

        public LoansProcessingTaskFactory(
            IClientRepository clientRepository,
            ILoanRepository loanRepository,
            ILoansProcessRepository loansProcessRepository)
        {
            _clientRepository = clientRepository;
            _loanRepository = loanRepository;
            _loansProcessRepository = loansProcessRepository;
        }

        public TaskDefinition Create(
            LoansProcess process)
        {
            var clientsCount = _clientRepository.CountInDataset(process.DatasetId);

            var batchSize = Client.DefaultReadingBatchSize;
            var batchesCount = clientsCount.CeilingOfDivisionBy(batchSize);
            var clientsReader = _clientRepository.GetBatchedReader(process.DatasetId, batchSize);

            return new TaskDefinition(
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
                            var loans = _loanRepository.GetRange(
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
