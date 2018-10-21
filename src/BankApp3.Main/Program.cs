using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using BankApp3.Main.ClientLoansCalculationTask;
using BankApp3.Main.MaxLossCalculationTask;
using BankApp3.Main.TotalLoanCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;
using DataProcessing.Utils.Settings;

namespace BankApp3.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var tasksToExecuteSettings = config.GetTasksToExecuteSettings();

            var depositSnapshotRepository = new DepositSnapshotRepository(connectionString);
            var loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
            var maxLossCalculationRepository = new MaxLossCalculationRepository(connectionString);
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(connectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(connectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(connectionString);
            
            var maxLossCalculationTaskFactory = new MaxLossCalculationTaskFactory(
                depositSnapshotRepository, 
                loanSnapshotRepository,
                maxLossCalculationRepository);

            var totalLoanCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                loanSnapshotRepository,
                totalLoanCalculationRepository);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                loanSnapshotRepository,
                clientLoansCalculationRepository,
                clientTotalLoanRepository);

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(connectionString).GetMaxId();

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
