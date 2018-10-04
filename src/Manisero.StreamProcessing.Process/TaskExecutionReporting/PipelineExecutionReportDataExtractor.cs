using System;
using System.Collections.Generic;
using System.Linq;
using Manisero.Navvy;
using Manisero.Navvy.Logging;
using Manisero.Navvy.PipelineProcessing;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    internal static class TaskLogReportingUtils
    {
        public static object GetLogValue(this TimeSpan timeSpan) => timeSpan.TotalMilliseconds;
        public static double ToMb(this long bytes) => bytes / 1024d / 1024d;
    }

    public class PipelineExecutionReportDataExtractor
    {
        const string MsUnitPart = " [ms]";
        const string MbUnitPart = " [mb]";
        const string MaterializationBlockName = "Materialization";

        public PipelineExecutionReportData Extract(
            TaskDefinition task,
            string pipelineStepName)
        {
            var log = task.GetExecutionLog();
            var stepLog = log.StepLogs[pipelineStepName];
            var step = task.Steps.Single(x => x.Name == pipelineStepName);
            var blockNames = ((IPipelineTaskStep)step).Blocks.Select(x => x.Name).ToArray();

            var itemTimesData = GetItemTimesData(stepLog, blockNames);
            var memoryData = GetMemoryData(stepLog, log.Diagnostics);
            var blockTimesData = GetBlockTimesData(stepLog, blockNames);

            return new PipelineExecutionReportData
            {
                ItemTimesData = itemTimesData.ToArray(),
                MemoryData = memoryData.ToArray(),
                BlockTimesData = blockTimesData.ToArray()
            };
        }

        private IEnumerable<ICollection<object>> GetItemTimesData(
            TaskStepLog stepLog,
            ICollection<string> blockNames)
        {
            var headerRow = new[] { "Item", "Step", "StartTs", "EndTs" };

            var dataRows = stepLog
                .ItemLogs
                .Select(x => GetItemTimesItemRows(x.Key, x.Value, blockNames, stepLog.Duration.StartTs))
                .SelectMany(rows => rows);

            return headerRow.ToEnumerable().Concat(dataRows);
        }

        private IEnumerable<ICollection<object>> GetItemTimesItemRows(
            int itemNumber,
            PipelineItemLog itemLog,
            ICollection<string> blockNames,
            DateTime stepStartTs)
        {
            var itemNumberString = $"Item {itemNumber}";

            yield return new[]
            {
                itemNumberString,
                MaterializationBlockName,
                (itemLog.Duration.StartTs - stepStartTs).GetLogValue(),
                (itemLog.Duration.StartTs + itemLog.MaterializationDuration - stepStartTs).GetLogValue()
            };

            foreach (var blockName in blockNames)
            {
                var blockDuration = itemLog.BlockDurations[blockName];

                yield return new[]
                {
                    itemNumberString,
                    blockName,
                    (blockDuration.StartTs - stepStartTs).GetLogValue(),
                    (blockDuration.EndTs - stepStartTs).GetLogValue()
                };
            }
        }

        private IEnumerable<ICollection<object>> GetMemoryData(
            TaskStepLog stepLog,
            IEnumerable<DiagnosticLog> diagnostics)
        {
            yield return new[] { "Time" + MsUnitPart, "Process working set" + MbUnitPart, "GC allocated set" + MbUnitPart };

            var stepStartTs = stepLog.Duration.StartTs;
            var stepEndTs = stepLog.Duration.EndTs;
            var relevantDiagnostics = diagnostics
                .Where(x => x.Timestamp >= stepStartTs && x.Timestamp <= stepEndTs)
                .OrderBy(x => x.Timestamp);

            foreach (var diagnostic in relevantDiagnostics)
            {
                yield return new[]
                {
                    (diagnostic.Timestamp - stepStartTs).GetLogValue(),
                    diagnostic.ProcessWorkingSet.ToMb(),
                    diagnostic.GcAllocatedSet.ToMb()
                };
            }
        }

        private IEnumerable<ICollection<object>> GetBlockTimesData(
            TaskStepLog stepLog,
            ICollection<string> blockNames)
        {
            yield return new[] { "Step", "Total duration" + MsUnitPart };
            yield return new[] { MaterializationBlockName, stepLog.BlockTotals.MaterializationDuration.GetLogValue() };

            foreach (var blockName in blockNames)
            {
                yield return new[] { blockName, stepLog.BlockTotals.BlockDurations[blockName].GetLogValue() };
            }
        }
    }
}
