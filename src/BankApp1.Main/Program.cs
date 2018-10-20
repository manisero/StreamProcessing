using BankApp1.Common.DataAccess;
using BankApp1.Main.ClientLoansCalculationTask;
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

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new DatasetRepository(connectionString),
                new ClientLoansCalculationRepository(connectionString),
                new ClientTotalLoanRepository(connectionString, processingSettings.UseBulkCopy));

            var datasetId = new DatasetRepository(connectionString).GetMaxId();;
            var task = clientLoansCalculationTaskFactory.Create(datasetId.Value);

            var taskResult = taskExecutor.Execute(task);
        }
    }
}
