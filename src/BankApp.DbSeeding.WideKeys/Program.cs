using BankApp.DataAccess.WideKeys;
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

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

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
