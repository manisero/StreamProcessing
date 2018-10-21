using Microsoft.Extensions.Configuration;

namespace DataProcessing.Utils.Settings
{
    public static class ConfigurationRootUtils
    {
        public static DbCreationSettings GetDbCreationSettings(
            this IConfigurationRoot config)
            => config.GetSection(nameof(DbCreationSettings)).BindAndReturn(new DbCreationSettings());

        public static DataSettings GetDataSettings(
            this IConfigurationRoot config)
            => config.GetSection(nameof(DataSettings)).BindAndReturn(new DataSettings());

        public static ProcessingSettings GetProcessingSettings(
            this IConfigurationRoot config)
            => config.GetSection(nameof(ProcessingSettings)).BindAndReturn(new ProcessingSettings());

        public static TasksToExecuteSettings GetTasksToExecuteSettings(
            this IConfigurationRoot config)
            => config.GetSection(nameof(TasksToExecuteSettings)).BindAndReturn(new TasksToExecuteSettings());
    }
}
