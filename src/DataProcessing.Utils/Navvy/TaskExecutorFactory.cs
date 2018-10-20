using System;
using System.IO;
using Manisero.Navvy;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;

namespace DataProcessing.Utils.Navvy
{
    public static class TaskExecutorFactory
    {
        public static ITaskExecutor Create(
            bool useDataflow = false)
        {
            var appName = AppDomainUtils.GetCurrentAppName();
            var reportsFolderPath = Path.Combine(Path.GetTempPath(), $"{appName}_reports");

            var builder = new TaskExecutorBuilder()
                .UseTaskExecutionLogger()
                .UseTaskExecutionReporter(x => Path.Combine(reportsFolderPath, x.Name))
                .RegisterEvents(
                    new TaskExecutionEvents(
                        taskStarted: x => Console.WriteLine($"Task {x.Task.Name} started."),
                        stepProgressed: x => Console.WriteLine($"{x.Step.Name}: {x.ProgressPercentage}%"),
                        taskEnded: x =>
                        {
                            Console.WriteLine($"Task took {x.Task.GetExecutionLog().TaskDuration.Duration.TotalMilliseconds} ms.");
                            Console.WriteLine($"Report written to: {x.Task.GetExecutionReportsPath()}");
                        }));

            if (useDataflow)
            {
                builder.UseDataflowPipelineExecution();
            }

            return builder.Build();
        }
    }
}
