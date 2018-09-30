using System;
using System.IO;
using System.Linq;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Process;
using Manisero.StreamProcessing.Process.DataAccess;
using Manisero.StreamProcessing.Process.LoansProcessing;
using Manisero.StreamProcessing.Process.TaskExecutionLogging;
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
            var progress = new Progress<TaskProgress>(
                x => Console.WriteLine($"{x.StepName}: {x.ProgressPercentage}%"));

            var taskResult = taskExecutor.Execute(loansProcessingTask, progress);

            Console.WriteLine($"Task took {TaskExecutionLog.Current.TaskDuration.Duration.TotalMilliseconds} ms.");

            var logFilePath = Path.GetTempFileName();

            new TaskStepLogWriter()
                .Write(
                    TaskExecutionLog.Current.StepLogs[loansProcessingTask.Steps.First().Name],
                    ((PipelineTaskStep<ClientsToProcess>)loansProcessingTask.Steps.First()).Blocks.Select(x => x.Name).ToArray(),
                    logFilePath);

            Console.WriteLine($"Log written to: {logFilePath}");
        }
    }
}
