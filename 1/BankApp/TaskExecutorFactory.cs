using Manisero.Navvy;
using Manisero.Navvy.Logging;

namespace BankApp
{
    public class TaskExecutorFactory
    {
        public ITaskExecutor Create()
        {
            return new TaskExecutorBuilder()
                .UseTaskExecutionLogger()
                .Build();
        }
    }
}
