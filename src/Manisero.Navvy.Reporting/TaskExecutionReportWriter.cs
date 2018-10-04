﻿using System;
using System.IO;
using System.Linq;
using Manisero.Navvy.Core.Events;
using Manisero.Navvy.Logging;
using Manisero.Navvy.PipelineProcessing;
using Manisero.Navvy.Reporting.PipelineReporting;

namespace Manisero.Navvy.Reporting
{
    public interface ITaskExecutionReportWriter
    {
        void Write(
            TaskDefinition task,
            string targetFolderPath);
    }

    public class TaskExecutionReportWriter : ITaskExecutionReportWriter
    {
        public static ITaskExecutionReportWriter Instance = new TaskExecutionReportWriter();

        private readonly IPipelineReportDataExtractor _pipelineReportDataExtractor = new PipelineReportDataExtractor();
        private readonly IPipelineReportWriter _pipelineReportWriter = new PipelineReportWriter();

        public void Write(
            TaskDefinition task,
            string targetFolderPath)
        {
            Directory.CreateDirectory(targetFolderPath);

            var log = task.GetExecutionLog();

            foreach (var pipeline in task.Steps.Where(x => x is IPipelineTaskStep).Cast<IPipelineTaskStep>())
            {
                WritePipelineReport(pipeline, log, targetFolderPath);
            }
        }

        private void WritePipelineReport(
            IPipelineTaskStep pipeline,
            TaskExecutionLog log,
            string taskReportFolderPath)
        {
            var data = _pipelineReportDataExtractor.Extract(pipeline, log);
            _pipelineReportWriter.Write(data, taskReportFolderPath);
        }

        public static TaskExecutionEvents CreateEvents(
            Func<TaskDefinition, string> reportFolderPathFactory)
        {
            return new TaskExecutionEvents(
                taskEnded: x =>
                {
                    var reportPath = reportFolderPathFactory(x.Task);

                    TaskExecutionReportWriter.Instance.Write(x.Task, reportPath);
                    x.Task.SetReportPath(reportPath);
                });
        }
    }
}