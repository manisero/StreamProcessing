using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.DataAccess.SurrogateKeys.Tasks;
using BankApp1.Main.ClientLoansCalculationTask;
using BankApp1.Main.MaxLossCalculationTask;
using BankApp1.Main.TotalLoanCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp1.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var datasetRepository = new DatasetRepository(settings.ConnectionString);
            var maxLossCalculationRepository = new MaxLossCalculationRepository(settings.ConnectionString);
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(settings.ConnectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(settings.ConnectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(settings.ConnectionString, settings.ProcessingSettings.UseBulkCopy);
            
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

            if (settings.TasksToExecuteSettings.MaxLossCalculation)
            {
                var maxLossCalculationTask = maxLossCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(maxLossCalculationTask);
            }

            if (settings.TasksToExecuteSettings.TotalLoanCalculation)
            {
                var totalLoanCalculationTask = totalLoanCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(totalLoanCalculationTask);
            }

            if (settings.TasksToExecuteSettings.ClientLoansCalculation)
            {
                var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
                taskExecutor.Execute(clientLoansCalculationTask);
            }
        }
    }
}
