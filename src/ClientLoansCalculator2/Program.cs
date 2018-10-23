using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using ClientLoansCalculator1;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace ClientLoansCalculator2
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();
            var taskExecutor = TaskExecutorFactory.Create();

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new LoanSnapshotRepository(settings.ConnectionString),
                new ClientLoansCalculationRepository(settings.ConnectionString),
                new ClientTotalLoanRepository(
                    settings.ConnectionString,
                    createUsingCopy: true));

            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var task = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(task);
        }
    }
}
