using System.Collections.Generic;
using System.Linq;
using Manisero.Navvy.Core;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class TaskExecutionLogger
    {
        public TaskExecutionLog Log { get; } = new TaskExecutionLog();

        public IExecutionEvents[] ExecutionEvents { get; }

        public TaskExecutionLogger()
        {
            ExecutionEvents = new IExecutionEvents[]
            {
                new TaskExecutionEvents(
                    taskStarted: x => Log.TaskDuration.StartTs = x.Timestamp,
                    taskEnded: x => Log.TaskDuration.SetEnd(x.Timestamp, x.Duration),
                    stepStarted: x => Log.StartStep(x.Step.Name, x.Timestamp),
                    stepEnded: x => Log.StepLogs[x.Step.Name].Duration.SetEnd(x.Timestamp, x.Duration)),
                new PipelineExecutionEvents(
                    itemMaterialized: x => Log.StepLogs[x.Step.Name].StartItem(x.ItemNumber, x.ItemStartTimestamp, x.MaterializationDuration),
                    itemEnded: x => Log.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].Duration.SetEnd(x.Timestamp, x.Duration),
                    blockStarted: x => Log.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].StartBlock(x.Block.Name, x.Timestamp),
                    blockEnded: x => Log.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].BlockDurations[x.Block.Name].SetEnd(x.Timestamp, x.Duration),
                    pipelineEnded: x => Log.StepLogs[x.Step.Name].BlockTotals = new TaskExecutionLog.BlockTotalsLog
                    {
                        MaterializationDuration = x.TotalInputMaterializationDuration,
                        BlockDurations = x.TotalBlockDurations.ToDictionary(entry => entry.Key, entry => entry.Value)
                    })
            };
        }
    }
}
