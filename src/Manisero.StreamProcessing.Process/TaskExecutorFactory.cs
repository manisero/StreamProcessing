using Manisero.Navvy;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.Logging;

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
