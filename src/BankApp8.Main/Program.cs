using System;
using System.IO;
using BankApp8.Common.DataAccess;
using BankApp8.Common.Domain;
using BankApp8.Main.ClientLoansCalculationTask;
using Manisero.Navvy;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;
using StreamProcessing.Utils;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            var reportsFolderPath = Path.Combine(Path.GetTempPath(), "StreamProcessing_reports");
            var taskExecutor = new TaskExecutorFactory().Create(reportsFolderPath);

            var clientRepository = new ClientRepository(connectionString);
            var loanRepository = new LoanRepository(connectionString);
            var loansProcessRepository = new LoansProcessRepository(connectionString);

            var loansProcessingTaskFactory = new LoansProcessingTaskFactory(
                clientRepository,
                loanRepository,
                loansProcessRepository);

            var process = loansProcessRepository.Create(new LoansProcess { DatasetId = 5 });
            var loansProcessingTask = loansProcessingTaskFactory.Create(process);
            var progress = new Progress<TaskProgress>(x => Console.WriteLine($"{x.StepName}: {x.ProgressPercentage}%"));
            
            var taskResult = taskExecutor.Execute(loansProcessingTask, progress);

            Console.WriteLine($"Task took {loansProcessingTask.GetExecutionLog().TaskDuration.Duration.TotalMilliseconds} ms.");
            Console.WriteLine($"Report written to: {loansProcessingTask.GetExecutionReportsPath()}");
        }
    }
}
