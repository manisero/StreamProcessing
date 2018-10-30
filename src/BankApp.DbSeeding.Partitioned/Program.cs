using BankApp.DataAccess.Partitioned.Data;
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

            var taskFactory = new DbSeedingTaskFactory(
                new DatabaseManager(settings.ConnectionString),
                new DatasetRepository(
                    settings.ConnectionString,
                    new DatabaseManager(settings.ConnectionString)),
                new ClientSnapshotRepository(settings.ConnectionString),
                new DepositSnapshotRepository(settings.ConnectionString),
                new LoanSnapshotRepository(settings.ConnectionString));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
