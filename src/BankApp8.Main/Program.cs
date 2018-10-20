using BankApp8.Common.DataAccess;
using BankApp8.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

            var taskExecutor = TaskExecutorFactory.Create();
            
            var loansProcessingTaskFactory = new LoansProcessingTaskFactory(
                new ClientSnapshotRepository(connectionString),
                new LoanSnapshotRepository(connectionString),
                new LoansProcessRepository(connectionString));

            var datasetId = new DatasetRepository(connectionString).GetMaxId();
            var task = loansProcessingTaskFactory.Create(datasetId.Value);
            
            var taskResult = taskExecutor.Execute(task);
        }
    }
}
