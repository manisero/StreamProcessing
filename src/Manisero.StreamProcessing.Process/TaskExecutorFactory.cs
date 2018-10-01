using System.Linq;
using Manisero.Navvy;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.PipelineProcessing.Events;
using Manisero.StreamProcessing.Process.TaskExecutionReporting;

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
            var taskEvents = new TaskExecutionEvents(
                taskStarted: x => TaskExecutionLog.Reset(x.Timestamp),
                taskEnded: x => TaskExecutionLog.Current.TaskDuration.SetEnd(x.Timestamp, x.Duration),
                stepStarted: x => TaskExecutionLog.Current.StartStep(x.Step.Name, x.Timestamp),
                stepEnded: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].Duration.SetEnd(x.Timestamp, x.Duration));

            var pipelineEvents = new PipelineExecutionEvents(
                itemMaterialized: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].StartItem(x.ItemNumber, x.ItemStartTimestamp, x.MaterializationDuration),
                itemEnded: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].Duration.SetEnd(x.Timestamp, x.Duration),
                blockStarted: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].StartBlock(x.Block.Name, x.Timestamp),
                blockEnded: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].BlockDurations[x.Block.Name].SetEnd(x.Timestamp, x.Duration),
                pipelineEnded: x => TaskExecutionLog.Current.StepLogs[x.Step.Name].BlockTotals = new TaskExecutionLog.BlockTotalsLog
                {
                    MaterializationDuration = x.TotalInputMaterializationDuration,
                    BlockDurations = x.TotalBlockDurations.ToDictionary(entry => entry.Key, entry => entry.Value)
                });

            return new TaskExecutorBuilder()
                .RegisterDataflowExecution()
                .RegisterEvents(taskEvents, pipelineEvents)
                .Build();
        }
    }
}
