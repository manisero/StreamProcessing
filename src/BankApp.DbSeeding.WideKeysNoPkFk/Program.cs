using BankApp.DataAccess.WideKeys;
using BankApp.DataAccess.WideKeys.Data;
using BankApp.DbSeeding.WideKeys;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.WideKeysNoPkFk
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(WideKeysNoPkFk));

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x),
                migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();

            var taskFactory = new DbSeedingTaskFactory(
                new DatasetRepository(settings.ConnectionString),
                new ClientSnapshotRepositoryWithSchema(settings.ConnectionString, hasPk: false, hasFk: false),
                new DepositSnapshotRepositoryWithSchema(settings.ConnectionString, hasPk: false, hasFk: false),
                new LoanSnapshotRepositoryWithSchema(settings.ConnectionString, hasPk: false, hasFk: false));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
