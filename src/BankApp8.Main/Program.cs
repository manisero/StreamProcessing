using System;
using System.IO;
using BankApp8.Common.DataAccess;
using BankApp8.Common.Domain;
using BankApp8.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

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
            
            var taskResult = taskExecutor.Execute(loansProcessingTask);

            Console.WriteLine($"Task took {loansProcessingTask.GetExecutionLog().TaskDuration.Duration.TotalMilliseconds} ms.");
            Console.WriteLine($"Report written to: {loansProcessingTask.GetExecutionReportsPath()}");
        }
    }
}
