using BankApp.DataAccess.Partitioned.Data;
using BankApp.DataAccess.Partitioned.Tasks;
using BankApp8.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;
using DataProcessing.Utils.Settings;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var tasksToExecuteSettings = config.GetTasksToExecuteSettings();

            var clientSnapshotRepository = new ClientSnapshotRepository(connectionString);
            var loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(connectionString);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                clientSnapshotRepository,
                loanSnapshotRepository,
                clientLoansCalculationRepository);

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(connectionString).GetMaxId();

            if (tasksToExecuteSettings.ClientLoansCalculation)
            {
                var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(clientLoansCalculationTask);
            }
        }
    }
}
