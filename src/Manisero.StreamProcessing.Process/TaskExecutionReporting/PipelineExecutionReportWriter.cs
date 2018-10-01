using System.IO;
using System.Linq;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class PipelineExecutionReportWriter
    {
        public void Write(
            PipelineExecutionReportData data,
            string targetFilePath)
        {
            var lines = EnumerableUtils.Concat(
                    data.ItemTimesData,
                    new[] { string.Empty }.ToEnumerable(),
                    data.BlockTimesData)
                .Select(x => x.Select(cell => cell.ToString()).JoinWithSeparator(", "));

            File.WriteAllLines(targetFilePath, lines);
        }
    }
}
