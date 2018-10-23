using BankApp.DataAccess.Partitioned.Data;
using BankApp.DataAccess.Partitioned.Tasks;
using BankApp8.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp8.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var clientSnapshotRepository = new ClientSnapshotRepository(settings.ConnectionString);
            var loanSnapshotRepository = new LoanSnapshotRepository(settings.ConnectionString);
            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(settings.ConnectionString);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                clientSnapshotRepository,
                loanSnapshotRepository,
                clientLoansCalculationRepository);

            var taskExecutor = TaskExecutorFactory.Create();
            var datasetId = new DatasetRepository(settings.ConnectionString).GetMaxId();

            var clientLoansCalculationTask = clientLoansCalculationTaskFactory.Create(datasetId.Value);
            taskExecutor.Execute(clientLoansCalculationTask);
        }
    }
}
