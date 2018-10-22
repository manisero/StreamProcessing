using BankApp.DataAccess.WideKeys;
using BankApp3.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp3.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();
            var seedingTask = new DbSeedingTaskFactory(settings.ConnectionString).Create(settings.DataSettings);

            taskExecutor.Execute(seedingTask);
        }
    }
}
