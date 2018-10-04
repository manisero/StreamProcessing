using Manisero.Navvy;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;

namespace Manisero.StreamProcessing.Process
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
                .RegisterDataflowExecution()
                .RegisterTaskExecutionLogger()
                .UseTaskExecutionReporting(x => reportsFolderPath)
                .Build();
        }
    }
}
