using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.Partitioned
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(Partitioned));

            var dbCreated = new DatabaseManager(settings.ConnectionString)
                .TryRecreate(
                    settings.DbCreationSettings.ForceRecreation,
                    migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();

            var task = new DbSeedingTaskFactory(settings.ConnectionString).Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
