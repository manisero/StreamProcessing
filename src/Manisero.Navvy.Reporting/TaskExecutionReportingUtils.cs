using System;

namespace Manisero.Navvy.Reporting
{
    public static class TaskExecutionReportingUtils
    {
        public const string TaskExecutionReportFolderPathExtraKey = "ReportPath";

        internal static void SetReportPath(
            this TaskDefinition task,
            string path)
            => task.Extras[TaskExecutionReportFolderPathExtraKey] = path;

        public static string GetReportPath(
            this TaskDefinition task)
            => (string)task.Extras[TaskExecutionReportFolderPathExtraKey];

        public static ITaskExecutorBuilder UseTaskExecutionReporting(
            this ITaskExecutorBuilder builder,
            Func<TaskDefinition, string> reportFolderPathFactory)
            => builder.RegisterEvents(TaskExecutionReportWriter.CreateEvents(reportFolderPathFactory));
    }
}
