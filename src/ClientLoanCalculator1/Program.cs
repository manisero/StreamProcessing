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
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            
            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new LoanSnapshotRepository(connectionString),
                new ClientLoansCalculationRepository(connectionString),
                new ClientTotalLoanRepository(connectionString));

            var taskExecutor = TaskExecutorFactory.Create();

            var datasetId = new DatasetRepository(connectionString).GetMaxId();

            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
