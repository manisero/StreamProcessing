using System.IO;
using System.Linq;
using BankApp.ClientLoansCalculationTask;
using BankApp.DataAccess;
using BankApp.Domain;
using StreamProcessing.Utils;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            var reportsFolderPath = Path.Combine(Path.GetTempPath(), "BankApp1_reports");

            var taskExecutor = new TaskExecutorFactory().Create(reportsFolderPath);

            var clientLoansCalculationTaskFactory = new ClientLoansCalculationTaskFactory(
                () => new EfContext(connectionString));

            var datasetId = GetDatasetId(connectionString);
            var task = clientLoansCalculationTaskFactory.Create(datasetId);

            var taskResult = taskExecutor.Execute(task);
        }

        private static int GetDatasetId(
            string connectionString)
        {
            using (var context = new EfContext(connectionString))
            {
                return context.Set<Dataset>().Select(x => x.DatasetId).Max();
            }
        }
    }
}
