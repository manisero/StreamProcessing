using Manisero.Navvy;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.StreamProcessing.Process
{
    public interface ITaskExecutorFactory
    {
        ITaskExecutor Create(
            TaskExecutionEvents taskEvents,
            PipelineExecutionEvents pipelineEvents);
    }

    public class TaskExecutorFactory : ITaskExecutorFactory
    {
        public ITaskExecutor Create(
            TaskExecutionEvents taskEvents,
            PipelineExecutionEvents pipelineEvents)
        {
            return new TaskExecutorBuilder()
                //.RegisterDataflowExecution()
                .RegisterEvents(taskEvents, pipelineEvents)
                .Build();
        }
    }
}
