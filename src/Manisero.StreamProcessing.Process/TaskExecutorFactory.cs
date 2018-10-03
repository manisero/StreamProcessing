using Manisero.Navvy;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Reporting;

namespace Manisero.StreamProcessing.Process
{
    public interface ITaskExecutorFactory
    {
        ITaskExecutor Create();
    }

    public class TaskExecutorFactory : ITaskExecutorFactory
    {
        public ITaskExecutor Create()
        {
            return new TaskExecutorBuilder()
                .RegisterDataflowExecution()
                .RegisterTaskExecutionLogger()
                .Build();
        }
    }
}
