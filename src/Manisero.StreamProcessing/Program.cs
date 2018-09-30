using System;
using Manisero.StreamProcessing.Domain;
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
            var loansProcessRepository = new LoansProcessRepository(connectionString);

            var taskExecutor = new TaskExecutorFactory().Create();

            var loansProcessingTaskFactory = new LoansProcessingTaskFactory(
                clientRepository,
                loanRepository,
                loansProcessRepository);

            var process = loansProcessRepository.Create(new LoansProcess { DatasetId = 5 });
            var loansProcessingTask = loansProcessingTaskFactory.Create(process);
            var taskResult = taskExecutor.Execute(loansProcessingTask);

            Console.WriteLine($"Task took {TaskExecutionLog.Current.TaskDuration.Duration.TotalMilliseconds} ms.");
        }
    }
}
