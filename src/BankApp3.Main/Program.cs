using BankApp3.Common.DataAccess.Data;
using BankApp3.Common.DataAccess.Tasks;
using BankApp3.Main.ClientLoansCalculationTask;
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
            
            var loanSnapshotRepository = new LoanSnapshotRepository(connectionString);
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(connectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(connectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(connectionString);
            
            var totalLoanCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                loanSnapshotRepository,
                totalLoanCalculationRepository);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                loanSnapshotRepository,
                clientLoansCalculationRepository,
                clientTotalLoanRepository);

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(connectionString).GetMaxId();

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
