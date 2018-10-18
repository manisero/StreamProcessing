using System.IO;
using Manisero.Navvy;
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
                .UseTaskExecutionReporter(x => Path.Combine(reportsFolderPath, x.Name));

            if (useDataflow)
            {
                builder.UseDataflowPipelineExecution();
            }

            return builder.Build();
        }
    }
}
