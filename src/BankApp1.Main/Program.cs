using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.DataAccess.SurrogateKeys.Tasks;
using BankApp1.Main.ClientLoansCalculationTask;
using BankApp1.Main.MaxLossCalculationTask;
using BankApp1.Main.TotalLoanCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;
using DataProcessing.Utils.Settings;

namespace BankApp1.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var processingSettings = config.GetProcessingSettings();
            var tasksToExecuteSettings = config.GetTasksToExecuteSettings();

            var datasetRepository = new DatasetRepository(connectionString);
            var maxLossCalculationRepository = new MaxLossCalculationRepository(connectionString);
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(connectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(connectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(connectionString, processingSettings.UseBulkCopy);
            
            var maxLossCalculationTaskFactory = new MaxLossCalculationTaskFactory(
                datasetRepository,
                maxLossCalculationRepository);

            var totalLoanCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                datasetRepository,
                totalLoanCalculationRepository);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                datasetRepository,
                clientLoansCalculationRepository,
                clientTotalLoanRepository);

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = datasetRepository.GetMaxId();

            if (tasksToExecuteSettings.MaxLossCalculation)
            {
                var maxLossCalculationTask = maxLossCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(maxLossCalculationTask);
            }

            if (tasksToExecuteSettings.TotalLoanCalculation)
            {
                var totalLoanCalculationTask = totalLoanCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(totalLoanCalculationTask);
            }

            if (tasksToExecuteSettings.ClientLoansCalculation)
            {
                var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(clientLoansCalculationTask);
            }
        }
    }
}
