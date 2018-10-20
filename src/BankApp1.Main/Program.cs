using BankApp1.Common.DataAccess;
using BankApp1.Main.ClientLoansCalculationTask;
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
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(connectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(connectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(connectionString, processingSettings.UseBulkCopy);

            var totalLoanCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                datasetRepository,
                totalLoanCalculationRepository);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                datasetRepository,
                clientLoansCalculationRepository,
                clientTotalLoanRepository);

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = datasetRepository.GetMaxId();

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
