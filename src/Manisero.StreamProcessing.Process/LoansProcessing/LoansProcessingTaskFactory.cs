﻿using System.Linq;
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
                        clientsReader.Enumerate().Select(x => new ClientsToProcess { Clients = x }),
                        batchesCount)
                    .WithBlock(
                        "LoadLoans",
                        x => x.Loans = _loanRepository.GetRange(
                            process.DatasetId,
                            x.Clients.First().ClientId,
                            x.Clients.Last().ClientId))
                    .WithBlock(
                        "SaveClientResults",
                        x => _loansProcessRepository.SaveClientResults(
                            x.Clients.Select(c => new LoansProcessClientResult
                            {
                                LoansProcessId = process.LoansProcessId,
                                ClientId = c.ClientId,
                                TotalLoan = 1m
                            })))
                    .Build());
        }
    }
}
