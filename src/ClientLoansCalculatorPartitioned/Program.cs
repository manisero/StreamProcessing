using BankApp.DataAccess.Partitioned.Data;
using BankApp.DataAccess.Partitioned.Tasks;
using ClientLoansCalculator1;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace ClientLoansCalculatorPartitioned
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();
            var databaseManager = new DatabaseManager(settings.ConnectionString);
            var taskExecutor = TaskExecutorFactory.Create();
            
            var taskFactory = new ClientLoansCalculationTaskFactory(
                new LoanSnapshotRepository(settings.ConnectionString),
                new ClientLoansCalculationRepository(settings.ConnectionString, databaseManager),
                new ClientTotalLoanRepository(settings.ConnectionString));

            var datasetId = new DatasetRepository(settings.ConnectionString, databaseManager)
                .GetMaxId();

            var task = taskFactory.Create(datasetId.Value);
            taskExecutor.Execute(task);
        }
    }
}
