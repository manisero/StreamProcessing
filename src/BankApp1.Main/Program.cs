using System.Linq;
using BankApp1.Common.DataAccess;
using BankApp1.Common.Domain;
using BankApp1.Main.ClientLoansCalculationTask;
using DataProcessing.Utils;
using DataProcessing.Utils.Navvy;

namespace BankApp1.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

            var taskExecutor = TaskExecutorFactory.Create();

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
