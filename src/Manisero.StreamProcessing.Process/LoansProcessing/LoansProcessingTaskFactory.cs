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
            short datasetId);
    }

    public class LoansProcessingTaskFactory : ILoansProcessingTaskFactory
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILoanRepository _loanRepository;

        public LoansProcessingTaskFactory(
            IClientRepository clientRepository,
            ILoanRepository loanRepository)
        {
            _clientRepository = clientRepository;
            _loanRepository = loanRepository;
        }

        public TaskDefinition Create(
            short datasetId)
        {
            var clientsCount = _clientRepository.CountInDataset(datasetId);

            var batchSize = Client.DefaultReadingBatchSize;
            var batchesCount = clientsCount.CeilingOfDivisionBy(batchSize);
            var clientsReader = _clientRepository.GetBatchedReader(datasetId, batchSize);

            return new TaskDefinition(
                PipelineTaskStep
                    .Builder<ClientsToProcess>("Process")
                    .WithInput(
                        clientsReader.Enumerate().Select(x => new ClientsToProcess { Clients = x }),
                        batchesCount)
                    .WithBlock(
                        "LoadLoans",
                        x => x.Loans = _loanRepository.GetRange(
                            datasetId,
                            x.Clients.First().ClientId,
                            x.Clients.Last().ClientId))
                    .Build());
        }
    }
}
