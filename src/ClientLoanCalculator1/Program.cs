using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace ClientLoanCalculator1
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new LoanSnapshotRepository(settings.ConnectionString),
                new ClientLoansCalculationRepository(settings.ConnectionString),
                new ClientTotalLoanRepository(settings.ConnectionString));

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
