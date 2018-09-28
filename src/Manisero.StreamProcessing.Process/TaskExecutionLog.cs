using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Manisero.StreamProcessing.Process
{
    public static class TaskExecutionLog
    {
        public class DurationLog
        {
            public DateTime StartTs { get; set; }

            public DateTime EndTs { get; set; }

            public TimeSpan Duration { get; set; }

            public void SetEnd(
                DateTime endTs,
                TimeSpan duration)
            {
                EndTs = endTs;
                Duration = duration;
            }
        }

        public class ItemLog
        {
            public DurationLog Duration { get; set; } = new DurationLog();

            public TimeSpan MaterializationDuration { get; set; }

            public ConcurrentDictionary<string, DurationLog> BlockDurations { get; set; } = new ConcurrentDictionary<string, DurationLog>();

            public void SetStart(
                DateTime startTs,
                TimeSpan materializationDuration)
            {
                Duration.StartTs = startTs;
                MaterializationDuration = materializationDuration;
            }

            public void StartBlock(
                string name,
                DateTime startTs)
            {
                BlockDurations.GetOrAdd(name, x => new DurationLog()).StartTs = startTs;
            }
        }

        public class BlockTotalsLog
        {
            public TimeSpan MaterializationDuration { get; set; }

            public Dictionary<string, TimeSpan> BlockDurations { get; set; }
        }

        public class StepLog
        {
            public DurationLog Duration { get; set; } = new DurationLog();

            /// <summary>ItemNumber -> Log</summary>
            public ConcurrentDictionary<int, ItemLog> ItemLogs { get; set; } = new ConcurrentDictionary<int, ItemLog>();

            public BlockTotalsLog BlockTotals { get; set; }

            public void StartItem(
                int number,
                DateTime startTs,
                TimeSpan materializationDuration)
            {
                ItemLogs.GetOrAdd(number, x => new ItemLog()).SetStart(startTs, materializationDuration);
            }
        }

        public static DurationLog TaskDuration { get; set; }

        /// <summary>StepName -> Log</summary>
        public static ConcurrentDictionary<string, StepLog> StepLogs { get; set; }

        public static void Reset(
            DateTime taskStartTs)
        {
            TaskDuration = new DurationLog { StartTs = taskStartTs };
            StepLogs = new ConcurrentDictionary<string, StepLog>();
        }

        public static void StartStep(
            string name,
            DateTime startTs)
        {
            StepLogs.GetOrAdd(name, x => new StepLog()).Duration.StartTs = startTs;
        }
    }
}
