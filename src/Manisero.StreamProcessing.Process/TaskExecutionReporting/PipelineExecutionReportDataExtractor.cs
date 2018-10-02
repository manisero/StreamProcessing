using System;
using System.Collections.Generic;
using System.Linq;
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
        const string UnitPart = " [ms]";
        const string MaterializationBlockName = "Materialization";

        public PipelineExecutionReportData Extract(
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames,
            ICollection<DiagnosticLog> diagnostics)
        {
            var itemTimesData = GetItemTimesData(stepLog, blockNames);
            var memoryData = GetMemoryData(stepLog, diagnostics);
            var blockTimesData = GetBlockTimesData(stepLog, blockNames);

            return new PipelineExecutionReportData
            {
                ItemTimesData = itemTimesData.ToArray(),
                MemoryData = memoryData.ToArray(),
                BlockTimesData = blockTimesData.ToArray()
            };
        }

        private IEnumerable<ICollection<object>> GetItemTimesData(
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames)
        {
            var itemsLogHeaderCells = GetItemTimesHeaderCells(blockNames).ToArray();

            var itemsLogItemsCells = stepLog
                .ItemLogs
                .Select(x => GetItemTimesItemCells(
                                x.Key,
                                x.Value,
                                blockNames,
                                stepLog.Duration.StartTs)
                            .ToArray());

            return itemsLogHeaderCells.ToEnumerable().Concat(itemsLogItemsCells);
        }

        private IEnumerable<string> GetItemTimesHeaderCells(
            ICollection<string> blockNames)
        {
            const string waitingHeader = "Waiting" + UnitPart;

            yield return "Item number";
            yield return waitingHeader;
            yield return MaterializationBlockName + UnitPart;

            foreach (var blockName in blockNames)
            {
                yield return waitingHeader;
                yield return blockName + UnitPart;
            }
        }

        private IEnumerable<object> GetItemTimesItemCells(
            int itemNumber,
            TaskExecutionLog.ItemLog itemLog,
            ICollection<string> blockNames,
            DateTime stepStartTs)
        {
            yield return itemNumber.ToString();

            yield return (itemLog.Duration.StartTs - stepStartTs).GetLogValue(); // Waiting before materialization
            yield return itemLog.MaterializationDuration.GetLogValue(); // Materialization

            var previousBlockEndTs = itemLog.Duration.StartTs + itemLog.MaterializationDuration;

            foreach (var blockName in blockNames)
            {
                var blockDuration = itemLog.BlockDurations[blockName];

                yield return (blockDuration.StartTs - previousBlockEndTs).GetLogValue(); // Waiting between blocks
                yield return (blockDuration.EndTs - blockDuration.StartTs).GetLogValue(); // Block

                previousBlockEndTs = blockDuration.EndTs;
            }
        }

        private IEnumerable<ICollection<object>> GetMemoryData(
            TaskExecutionLog.StepLog stepLog,
            ICollection<DiagnosticLog> diagnostics)
        {
            yield return new[] { "Time" + UnitPart, "Process working set [MB]", "GC allocated set [MB]" };

            var stepStartTs = stepLog.Duration.StartTs;
            var stepEndTs = stepLog.Duration.EndTs;
            var relevantDiagnostics = diagnostics.Where(x => x.Timestamp >= stepStartTs && x.Timestamp <= stepEndTs);

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
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames)
        {
            yield return new[] { "Step", "Total duration" + UnitPart };
            yield return new[] { MaterializationBlockName, stepLog.BlockTotals.MaterializationDuration.GetLogValue() };

            foreach (var blockName in blockNames)
            {
                yield return new[] { blockName, stepLog.BlockTotals.BlockDurations[blockName].GetLogValue() };
            }
        }
    }
}
