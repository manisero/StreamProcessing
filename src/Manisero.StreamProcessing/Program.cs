using System.Linq;
using Manisero.StreamProcessing.Process;
using Manisero.StreamProcessing.Process.DataAccess;
using Manisero.StreamProcessing.Process.LoansProcessing;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            var clientRepository = new ClientRepository(connectionString);
            var clientCount = clientRepository.CountInDataset(5);
            var clientReader = clientRepository.GetBatchedReader(5);
            var clientsBatch1 = clientReader.ReadNext();
            var clientsBatch2 = clientReader.ReadNext();

            var loanRepository = new LoanRepository(connectionString);
            var loanReader = loanRepository.GetBatchedReader(5);
            var loansBatch1 = loanReader.ReadNext();
            var loansBatch2 = loanReader.ReadNext();

            var loansRange = loanRepository.GetRange(5, clientsBatch2.First().ClientId, clientsBatch2.Last().ClientId);

            var taskExecutorFactory = new TaskExecutorFactory();
            var taskExecutor = taskExecutorFactory.Create();

            var loansProcessingTaskFactory = new LoansProcessingTaskFactory(
                clientRepository,
                loanRepository);

            var loansProcessingTask = loansProcessingTaskFactory.Create(5);
            var taskResult = taskExecutor.Execute(loansProcessingTask);
        }
    }
}
