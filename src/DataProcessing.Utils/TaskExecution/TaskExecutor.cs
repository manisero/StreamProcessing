using DataProcessing.Utils.Navvy;

namespace DataProcessing.Utils.TaskExecution
{
    public static class TaskExecutor
    {
        public static void Execute(
            short datasetId)
        {
            var taskExecutor = TaskExecutorFactory.Create();
        }
    }
}
