using BankApp.DataAccess.SurrogateKeys.Data;
using BankApp.DataAccess.SurrogateKeys.Tasks;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace TotalLoanCalculator1
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();
            var taskExecutor = TaskExecutorFactory.Create();

            var clientLoansCalculationTaskFactory = new TotalLoanCalculationTaskFactory(
                new DatasetRepository(settings.ConnectionString),
                new TotalLoanCalculationRepository(settings.ConnectionString));

            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var task = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(task);
        }
    }
}
