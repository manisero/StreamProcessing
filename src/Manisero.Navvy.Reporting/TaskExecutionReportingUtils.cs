﻿using System;
using System.Collections.Generic;

namespace Manisero.Navvy.Reporting
{
    public static class TaskExecutionReportingUtils
    {
        public const string TaskExecutionReportsExtraKey = "ExecutionReports";
        public const string TaskExecutionReportsFolderPathExtraKey = "ExecutionReportsPath";

        internal static void SetExecutionReports(
            this TaskDefinition task,
            IReadOnlyCollection<TaskExecutionReport> reports)
            => task.Extras[TaskExecutionReportsExtraKey] = reports;

        public static IReadOnlyCollection<TaskExecutionReport> GetExecutionReports(
            this TaskDefinition task)
            => (IReadOnlyCollection<TaskExecutionReport>)task.Extras[TaskExecutionReportsExtraKey];

        internal static void SetExecutionReportsPath(
            this TaskDefinition task,
            string path)
            => task.Extras[TaskExecutionReportsFolderPathExtraKey] = path;

        public static string GetExecutionReportsPath(
            this TaskDefinition task)
            => (string)task.Extras[TaskExecutionReportsFolderPathExtraKey];

        public static ITaskExecutorBuilder UseTaskExecutionReporting(
            this ITaskExecutorBuilder builder,
            Func<TaskDefinition, string> reportsFolderPathFactory = null)
            => builder.RegisterEvents(TaskExecutionReportsGenerator.CreateEvents(reportsFolderPathFactory));
    }
}
