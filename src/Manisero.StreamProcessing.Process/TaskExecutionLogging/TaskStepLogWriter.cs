using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.TaskExecutionLogging
{
    internal static class TaskLogWritingUtils
    {
        public static string FormatLog(this TimeSpan timeSpan) => timeSpan.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
    }

    public class TaskStepLogWriter
    {
        const string UnitPart = " [ms]";
        const string MaterializationBlockName = "Materialization";

        public void Write(
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames,
            string targetFilePath)
        {
            var itemsLogHeaderCells = GetItemsLogHeaderCells(blockNames);
            var itemsLogItemsCells = stepLog.ItemLogs.Select(x => GetItemsLogItemCells(x.Key, x.Value, blockNames, stepLog.Duration.StartTs));

            var totalDurationsReport = GetTotalDurationsReport(stepLog, blockNames);

            var lines = EnumerableUtils.Concat(
                    itemsLogHeaderCells.ToEnumerable(),
                    itemsLogItemsCells,
                    string.Empty.ToEnumerable().ToEnumerable(),
                    totalDurationsReport)
                .Select(x => x.JoinWithSeparator(", "));

            File.WriteAllLines(targetFilePath, lines);
        }

        private IEnumerable<string> GetItemsLogHeaderCells(
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

        private IEnumerable<string> GetItemsLogItemCells(
            int itemNumber,
            TaskExecutionLog.ItemLog itemLog,
            ICollection<string> blockNames,
            DateTime stepStartTs)
        {
            yield return itemNumber.ToString();

            yield return (itemLog.Duration.StartTs - stepStartTs).FormatLog(); // Waiting before materialization
            yield return itemLog.MaterializationDuration.FormatLog(); // Materialization

            var previousBlockEndTs = itemLog.Duration.StartTs + itemLog.MaterializationDuration;

            foreach (var blockName in blockNames)
            {
                var blockDuration = itemLog.BlockDurations[blockName];

                yield return (blockDuration.StartTs - previousBlockEndTs).FormatLog(); // Waiting between blocks
                yield return blockDuration.Duration.FormatLog(); // Block

                previousBlockEndTs = blockDuration.StartTs + blockDuration.Duration;
            }
        }

        private IEnumerable<IEnumerable<string>> GetTotalDurationsReport(
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames)
        {
            yield return new[] { "Step", "Total duration" + UnitPart };
            yield return new[] { MaterializationBlockName, stepLog.BlockTotals.MaterializationDuration.FormatLog() };

            foreach (var blockName in blockNames)
            {
                yield return new[] { blockName, stepLog.BlockTotals.BlockDurations[blockName].FormatLog() };
            }
        }
    }
}
