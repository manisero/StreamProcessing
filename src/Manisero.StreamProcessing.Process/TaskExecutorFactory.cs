using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Manisero.Navvy;
using Manisero.Navvy.Core;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Dataflow;
using Manisero.Navvy.PipelineProcessing.Events;

namespace Manisero.StreamProcessing.Process
{
    public interface ITaskExecutorFactory
    {
        ITaskExecutor Create(
            params IExecutionEvents[] events);
    }

    public class TaskExecutorFactory : ITaskExecutorFactory
    {
        public ITaskExecutor Create(
            params IExecutionEvents[] events)
        {
            var taskEvents = new TaskExecutionEvents(
                taskStarted: x => TaskExecutionLog.Reset(x.Timestamp),
                taskEnded: x => TaskExecutionLog.TaskDuration.EndTs = x.Timestamp,
                stepStarted: x => TaskExecutionLog.StartStep(x.Step.Name, x.Timestamp),
                stepEnded: x => TaskExecutionLog.StepLogs[x.Step.Name].Duration.SetEnd(x.Timestamp, x.Duration));

            var pipelineEvents = new PipelineExecutionEvents(
                itemStarted: x => TaskExecutionLog.StepLogs[x.Step.Name].StartItem(x.ItemNumber, x.Timestamp, x.MaterializationDuration),
                itemEnded: x => TaskExecutionLog.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].Duration.SetEnd(x.Timestamp, x.Duration),
                blockStarted: x => TaskExecutionLog.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].StartBlock(x.Block.Name, x.Timestamp),
                blockEnded: x => TaskExecutionLog.StepLogs[x.Step.Name].ItemLogs[x.ItemNumber].BlockDurations[x.Block.Name].SetEnd(x.Timestamp, x.Duration),
                pipelineEnded: x => TaskExecutionLog.StepLogs[x.Step.Name].BlockTotals = new TaskExecutionLog.BlockTotalsLog
                {
                    MaterializationDuration = x.TotalInputMaterializationDuration,
                    BlockDurations = x.TotalBlockDurations.ToDictionary(entry => entry.Key, entry => entry.Value)
                });

            return new TaskExecutorBuilder()
                //.RegisterDataflowExecution()
                .RegisterEvents(events)
                .Build();
        }
    }
}
