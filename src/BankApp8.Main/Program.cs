using BankApp.DataAccess.Partitioned.Data;
using BankApp.DataAccess.Partitioned.Tasks;
using BankApp8.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();
            var databaseManager = new DatabaseManager(settings.ConnectionString);

            var clientSnapshotRepository = new ClientSnapshotRepository(settings.ConnectionString);
            var loanSnapshotRepository = new LoanSnapshotRepository(settings.ConnectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(
                settings.ConnectionString,
                databaseManager);
            var clientTotalLoanRepository = new ClientTotalLoanRepository(settings.ConnectionString);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                clientSnapshotRepository,
                loanSnapshotRepository,
                clientLoansCalculationRepository,
                clientTotalLoanRepository);

            var taskExecutor = TaskExecutorFactory.Create();
            var datasetId = new DatasetRepository(
                    settings.ConnectionString,
                    databaseManager)
                .GetMaxId();

            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
