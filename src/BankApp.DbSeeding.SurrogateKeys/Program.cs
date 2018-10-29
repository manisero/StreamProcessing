using BankApp.DataAccess.SurrogateKeys;
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

            var task = new DbSeedingTaskFactory(settings.ConnectionString).Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
