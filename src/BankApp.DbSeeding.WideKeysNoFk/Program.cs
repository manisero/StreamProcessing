using BankApp.DataAccess.WideKeys.Data;
using BankApp.DbSeeding.WideKeys;
using BankApp.Domain.WideKeys;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.WideKeysNoFk
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(WideKeysNoFk));
            var databaseManager = new DatabaseManager(settings.ConnectionString);

            var dbCreated = databaseManager.TryRecreate(
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x),
                migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();

            var taskFactory = new DbSeedingTaskFactory(
                new DatabaseManager(settings.ConnectionString),
                new DatasetRepository(settings.ConnectionString),
                new ClientSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: false),
                new DepositSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: false),
                new LoanSnapshotRepositoryWithSchema(settings.ConnectionString, databaseManager, hasPk: true, hasFk: false));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
