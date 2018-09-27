using Manisero.Navvy;
using Manisero.Navvy.Dataflow;

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
                //.RegisterDataflowExecution()
                .Build();
        }
    }
}
