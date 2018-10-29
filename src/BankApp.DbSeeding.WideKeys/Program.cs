using BankApp.DataAccess.WideKeys;
using BankApp.DataAccess.WideKeys.Data;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.WideKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(WideKeys));
            var databaseManager = new DatabaseManager(settings.ConnectionString);

            var dbCreated = databaseManager.TryRecreate(
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();
            
            var taskFactory = new DbSeedingTaskFactory(
                new DatasetRepository(settings.ConnectionString),
                new ClientSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: true),
                new DepositSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: true),
                new LoanSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: true));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
