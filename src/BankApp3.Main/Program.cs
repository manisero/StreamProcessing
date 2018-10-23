using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using BankApp3.Main.ClientLoansCalculationTask;
using BankApp3.Main.MaxLossCalculationTask;
using BankApp3.Main.TotalLoanCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp3.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var depositSnapshotRepository = new DepositSnapshotRepository(settings.ConnectionString);
            var loanSnapshotRepository = new LoanSnapshotRepository(settings.ConnectionString);
            var maxLossCalculationRepository = new MaxLossCalculationRepository(settings.ConnectionString);
            var totalLoanCalculationRepository = new TotalLoanCalculationRepository(settings.ConnectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(settings.ConnectionString);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(settings.ConnectionString);

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
            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var maxLossCalculationTask = maxLossCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(maxLossCalculationTask);

            var totalLoanCalculationTask = totalLoanCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(totalLoanCalculationTask);

            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
