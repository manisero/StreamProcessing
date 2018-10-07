using System.IO;
using BankApp.CalculateClientLoans;
using BankApp.DataAccess;
using BankApp.Utils;

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

            var calculateClientLoansTaskFactory = new CalculateClientLoansTaskFactory(
                () => new EfContext(connectionString));

            var task = calculateClientLoansTaskFactory.Create(2);

            var taskResult = taskExecutor.Execute(task);
        }
    }
}
