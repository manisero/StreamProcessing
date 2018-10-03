using System;
using System.IO;
using Manisero.Navvy;
using Manisero.Navvy.Reporting;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Process;
using Manisero.StreamProcessing.Process.DataAccess;
using Manisero.StreamProcessing.Process.LoansProcessing;
using Manisero.StreamProcessing.Process.TaskExecutionReporting;
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
            var progress = new Progress<TaskProgress>(x => Console.WriteLine($"{x.StepName}: {x.ProgressPercentage}%"));
            
            var taskResult = taskExecutor.Execute(loansProcessingTask, progress);

            Console.WriteLine($"Task took {loansProcessingTask.GetExecutionLog().TaskDuration.Duration.TotalMilliseconds} ms.");
            
            var reportData = new PipelineExecutionReportDataExtractor()
                .Extract(loansProcessingTask, loansProcessingTask.Steps[0].Name);

            var reportFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            new PipelineExecutionReportWriter()
                .Write(reportData, reportFolderPath);

            Console.WriteLine($"Report written to: {reportFolderPath}");
        }
    }
}
