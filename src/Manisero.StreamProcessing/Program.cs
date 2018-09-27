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
            var loanRepository = new LoanRepository(connectionString);

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
