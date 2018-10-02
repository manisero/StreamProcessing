using System.Collections.Generic;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class PipelineExecutionReportData
    {
        public ICollection<ICollection<object>> ItemTimesData { get; set; }
        public ICollection<ICollection<object>> MemoryData { get; set; }
        public ICollection<ICollection<object>> BlockTimesData { get; set; }
    }
}
