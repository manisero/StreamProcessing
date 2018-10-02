﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class TaskExecutionLog
    {
        public class DurationLog
        {
            public DateTime StartTs { get; set; }

            public DateTime EndTs { get; private set; }

            public TimeSpan Duration { get; private set; }

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
            public DurationLog Duration { get; } = new DurationLog();

            public TimeSpan MaterializationDuration { get; private set; }

            public ConcurrentDictionary<string, DurationLog> BlockDurations { get; } = new ConcurrentDictionary<string, DurationLog>();

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
            public DurationLog Duration { get; } = new DurationLog();

            /// <summary>ItemNumber -> Log</summary>
            public ConcurrentDictionary<int, ItemLog> ItemLogs { get; } = new ConcurrentDictionary<int, ItemLog>();

            public BlockTotalsLog BlockTotals { get; set; }

            public void StartItem(
                int number,
                DateTime startTs,
                TimeSpan materializationDuration)
            {
                ItemLogs.GetOrAdd(number, x => new ItemLog()).SetStart(startTs, materializationDuration);
            }
        }

        public DurationLog TaskDuration { get; } = new DurationLog();

        /// <summary>StepName -> Log</summary>
        public ConcurrentDictionary<string, StepLog> StepLogs { get; } = new ConcurrentDictionary<string, StepLog>();

        public void StartStep(
            string name,
            DateTime startTs)
        {
            StepLogs.GetOrAdd(name, x => new StepLog()).Duration.StartTs = startTs;
        }
    }
}
