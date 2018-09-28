using System;
using Manisero.Navvy;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.PipelineProcessing.Events;
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

            var taskExecutor = CreateTaskExecutor();

            var loansProcessingTaskFactory = new LoansProcessingTaskFactory(
                clientRepository,
                loanRepository,
                loansProcessRepository);

            var process = loansProcessRepository.Create(new LoansProcess { DatasetId = 5 });
            var loansProcessingTask = loansProcessingTaskFactory.Create(process);
            var taskResult = taskExecutor.Execute(loansProcessingTask);
        }

        private static ITaskExecutor CreateTaskExecutor()
        {
            var taskEvents = new TaskExecutionEvents(
                taskStarted: x => Console.WriteLine("Task started."),
                taskEnded: x => Console.WriteLine($"Task ended after {x.Duration.TotalMilliseconds}ms."),
                stepStarted: x => Console.WriteLine($"{x.Step.Name}:"),
                stepEnded: x => Console.WriteLine($"{x.Step.Name} took {x.Duration.TotalMilliseconds}ms."),
                stepSkipped: x => Console.WriteLine($"{x.Step.Name} skipped"),
                stepCanceled: x => Console.WriteLine($"{x.Step.Name} canceled"),
                stepFailed: x => Console.WriteLine($"{x.Step.Name} failed"));

            var pipelineEvents = new PipelineExecutionEvents(
                itemStarted: x => Console.WriteLine($"  Item {x.ItemNumber} (materialized in {x.MaterializationDuration.TotalMilliseconds}ms):"),
                itemEnded: x => Console.WriteLine($"  Item {x.ItemNumber} ended after {x.Duration.TotalMilliseconds}ms."),
                blockStarted: x => Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber}..."),
                blockEnded: x => Console.WriteLine($"    {x.Block.Name} of {x.ItemNumber} took {x.Duration.TotalMilliseconds}ms."),
                pipelineEnded: x =>
                {
                    Console.WriteLine($"      Materialization: {x.TotalInputMaterializationDuration.TotalMilliseconds}ms");

                    foreach (var blockDuration in x.TotalBlockDurations)
                    {
                        Console.WriteLine($"      {blockDuration.Key}: {blockDuration.Value.TotalMilliseconds}ms");
                    }
                });

            return new TaskExecutorFactory().Create(taskEvents, pipelineEvents);
        }
    }
}
