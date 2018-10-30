using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.Domain.SurrogateKeys;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.SurrogateKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(SurrogateKeys));

            var dbCreated = new DatabaseManager(settings.ConnectionString)
                .TryRecreate(
                    settings.DbCreationSettings.ForceRecreation,
                    efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();

            var taskFactory = new DbSeedingTaskFactory(
                new DatabaseManager(settings.ConnectionString),
                new DatasetRepository(settings.ConnectionString),
                new ClientSnapshotRepository(settings.ConnectionString),
                new LoanSnapshotRepository(settings.ConnectionString));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
