using System.Collections.Generic;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Process.DataAccess;

namespace Manisero.StreamProcessing.Process.LoansProcessing
{
    public class ClientsToProcess
    {
        public ICollection<Client> Clients { get; set; }
        
        /// <summary>ClientId -> Loans</summary>
        public IDictionary<int, ICollection<Loan>> Loans { get; set; }
    }

    public class LoansProcessingTaskFactory
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
            var batchSize = Client.DefaultReadingBatchSize;
            var clientsCount = _clientRepository.CountInDataset(datasetId);
            var clientsReader = _clientRepository.GetBatchedReader(datasetId, batchSize);

            return new TaskDefinition(
                PipelineTaskStep
                    .Builder<ClientsToProcess>("Process")
                    .Build());
        }
    }
}
