using System.IO;
using Manisero.Navvy;
using Manisero.Navvy.Logging;
using Manisero.Navvy.Reporting;

namespace BankApp
{
    public class TaskExecutorFactory
    {
        public ITaskExecutor Create(
            string reportsFolderPath)
        {
            return new TaskExecutorBuilder()
                .UseTaskExecutionLogger()
                .UseTaskExecutionReporter(x => Path.Combine(reportsFolderPath, x.Name))
                .Build();
        }
    }
}
