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
        public void Write(
            TaskExecutionLog.StepLog stepLog,
            ICollection<string> blockNames,
            string targetFilePath)
        {
            var headerCells = GetHeaderCells(blockNames);
            var itemsCells = stepLog.ItemLogs.Select(x => GetItemCells(x.Key, x.Value, blockNames, stepLog.Duration.StartTs));

            var lines = headerCells.ToEnumerable().Concat(itemsCells).Select(x => x.JoinWithSeparator(", "));

            File.WriteAllLines(targetFilePath, lines);
        }

        public IEnumerable<string> GetHeaderCells(
            ICollection<string> blockNames)
        {
            const string unitPart = " [ms]";
            const string waitingHeader = "Waiting" + unitPart;

            yield return "Item number";
            yield return waitingHeader;
            yield return "Materialization" + unitPart;

            foreach (var blockName in blockNames)
            {
                yield return waitingHeader;
                yield return blockName + unitPart;
            }
        }

        public IEnumerable<string> GetItemCells(
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
    }
}
