using System.Linq;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.Navvy.Reporting
{
    public class TaskExecutionLogger
    {
        public void HandleTaskStarted(
            TaskStartedEvent e, TaskExecutionLog log)
        {
            log.Task = e.Task;
            log.TaskDuration.SetStart(e.Timestamp);
        }

        public void HandleTaskEnded(
            TaskEndedEvent e, TaskExecutionLog log)
        {
            log.TaskDuration.SetEnd(e.Timestamp, e.Duration);
        }

        public void HandleStepStarted(
            StepStartedEvent e, TaskExecutionLog log)
        {
            var stepLog = new TaskStepLog();
            stepLog.Duration.SetStart(e.Timestamp);
            log.StepLogs[e.Step.Name] = stepLog;
        }

        public void HandleStepEnded(
            StepEndedEvent e, TaskExecutionLog log)
        {
            log.StepLogs[e.Step.Name].Duration.SetEnd(e.Timestamp, e.Duration);
        }

        public void HandleItemMaterialized(
            ItemMaterializedEvent e, TaskExecutionLog log)
        {
            var itemLog = new PipelineItemLog();
            itemLog.Duration.SetStart(e.ItemStartTimestamp);
            itemLog.MaterializationDuration = e.MaterializationDuration;
            
            log.StepLogs[e.Step.Name].ItemLogs[e.ItemNumber] = itemLog;
        }

        public void HandleItemEnded(
            ItemEndedEvent e, TaskExecutionLog log)
        {
            log.StepLogs[e.Step.Name].ItemLogs[e.ItemNumber].Duration.SetEnd(e.Timestamp, e.Duration);
        }

        public void HandleBlockStarted(
            BlockStartedEvent e, TaskExecutionLog log)
        {
            var blockLog = new DurationLog();
            blockLog.SetStart(e.Timestamp);

            log.StepLogs[e.Step.Name].ItemLogs[e.ItemNumber].BlockDurations[e.Block.Name] = blockLog;
        }

        public void HandleBlockEnded(
            BlockEndedEvent e, TaskExecutionLog log)
        {
            log.StepLogs[e.Step.Name].ItemLogs[e.ItemNumber].BlockDurations[e.Block.Name].SetEnd(e.Timestamp, e.Duration);
        }

        public void HandlePipelineEnded(
            PipelineEndedEvent e, TaskExecutionLog log)
        {
            log.StepLogs[e.Step.Name].BlockTotals = new PipelineBlockTotalsLog
            {
                MaterializationDuration = e.TotalInputMaterializationDuration,
                BlockDurations = e.TotalBlockDurations.ToDictionary(entry => entry.Key, entry => entry.Value)
            };
        }
    }
}
