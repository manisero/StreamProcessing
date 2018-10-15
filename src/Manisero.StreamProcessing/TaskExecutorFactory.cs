using System.IO;
using Manisero.Navvy;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;

namespace Manisero.StreamProcessing
{
    public interface ITaskExecutorFactory
    {
        ITaskExecutor Create(
            string reportsFolderPath);
    }

    public class TaskExecutorFactory : ITaskExecutorFactory
    {
        public ITaskExecutor Create(
            string reportsFolderPath)
        {
            return new TaskExecutorBuilder()
                .UseDataflowPipelineExecution()
                .UseTaskExecutionLogger()
                .UseTaskExecutionReporter(x => Path.Combine(reportsFolderPath, x.Name))
                .Build();
        }
    }
}
