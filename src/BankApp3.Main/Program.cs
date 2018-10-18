using BankApp3.Common.DataAccess;
using BankApp3.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp3.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

            var taskExecutor = TaskExecutorFactory.Create();

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                new ClientLoansCalculationRepository(connectionString),
                new LoanSnapshotRepository(connectionString),
                new ClientTotalLoanRepository(connectionString));

            var datasetId = new DatasetRepository(connectionString).GetMaxId();
            var task = clientLoansCalculationTaskFactory.Create(datasetId.Value);

            var taskResult = taskExecutor.Execute(task);
        }
    }
}
