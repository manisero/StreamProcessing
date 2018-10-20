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

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(connectionString).GetMaxId();

            var totalLoanCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                new DatasetRepository(connectionString),
                new TotalLoanCalculationRepository(connectionString));

            var totalLoanCalculationTask = totalLoanCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(totalLoanCalculationTask);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new DatasetRepository(connectionString),
                new ClientLoansCalculationRepository(connectionString),
                new ClientTotalLoanRepository(connectionString, processingSettings.UseBulkCopy));
            
            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
