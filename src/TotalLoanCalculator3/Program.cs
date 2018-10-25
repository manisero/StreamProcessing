using BankApp.DataAccess.WideKeys.Data;
using BankApp.DataAccess.WideKeys.Tasks;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace TotalLoanCalculator3
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();
            var taskExecutor = TaskExecutorFactory.Create();

            var taskFactory = new TotalLoanCalculationTaskFactory(
                new LoanSnapshotRepository(
                    settings.ConnectionString,
                    readUsingDapper: false),
                new TotalLoanCalculationRepository(settings.ConnectionString));

            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var task = taskFactory.Create(datasetId.Value);
            taskExecutor.Execute(task);
        }
    }
}
