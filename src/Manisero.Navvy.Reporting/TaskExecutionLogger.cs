using System.Linq;
using Manisero.Navvy.Core;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.Navvy.Reporting
{
    public static class TaskExecutionLogger
    {
        public static IExecutionEvents[] CreateEvents()
        {
            return new IExecutionEvents[]
            {
                new TaskExecutionEvents(
                    taskStarted: HandleTaskStarted,
                    taskEnded: HandleTaskEnded,
                    stepStarted: HandleStepStarted,
                    stepEnded: HandleStepEnded),
                new PipelineExecutionEvents(
                    itemMaterialized: HandleItemMaterialized,
                    itemEnded: HandleItemEnded,
                    blockStarted: HandleBlockStarted,
                    blockEnded: HandleBlockEnded,
                    pipelineEnded: HandlePipelineEnded)
            };
        }

        public static void HandleTaskStarted(
            TaskStartedEvent e)
        {
            var log = new TaskExecutionLog();
            log.TaskDuration.SetStart(e.Timestamp);

            e.Task.SetExecutionLog(log);
        }

        public static void HandleTaskEnded(
            TaskEndedEvent e)
        {
            e.Task.GetExecutionLog().TaskDuration.SetEnd(e.Timestamp, e.Duration);
        }

        public static void HandleStepStarted(
            StepStartedEvent e)
        {
            var stepLog = new TaskStepLog();
            stepLog.Duration.SetStart(e.Timestamp);

            e.Task.GetExecutionLog().StepLogs[e.Step.Name] = stepLog;
        }

        public static void HandleStepEnded(
            StepEndedEvent e)
        {
            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .Duration
                .SetEnd(e.Timestamp, e.Duration);
        }

        public static void HandleItemMaterialized(
            ItemMaterializedEvent e)
        {
            var itemLog = new PipelineItemLog();
            itemLog.Duration.SetStart(e.ItemStartTimestamp);
            itemLog.MaterializationDuration = e.MaterializationDuration;

            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .ItemLogs[e.ItemNumber] = itemLog;
        }

        public static void HandleItemEnded(
            ItemEndedEvent e)
        {
            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .ItemLogs[e.ItemNumber]
                .Duration
                .SetEnd(e.Timestamp, e.Duration);
        }

        public static void HandleBlockStarted(
            BlockStartedEvent e)
        {
            var blockLog = new DurationLog();
            blockLog.SetStart(e.Timestamp);

            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .ItemLogs[e.ItemNumber]
                .BlockDurations[e.Block.Name] = blockLog;
        }

        public static void HandleBlockEnded(
            BlockEndedEvent e)
        {
            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .ItemLogs[e.ItemNumber]
                .BlockDurations[e.Block.Name]
                .SetEnd(e.Timestamp, e.Duration);
        }

        public static void HandlePipelineEnded(
            PipelineEndedEvent e)
        {
            e.Task.GetExecutionLog()
                .StepLogs[e.Step.Name]
                .BlockTotals = new PipelineBlockTotalsLog
            {
                MaterializationDuration = e.TotalInputMaterializationDuration,
                BlockDurations = e.TotalBlockDurations.ToDictionary(entry => entry.Key, entry => entry.Value)
            };
        }
    }
}
