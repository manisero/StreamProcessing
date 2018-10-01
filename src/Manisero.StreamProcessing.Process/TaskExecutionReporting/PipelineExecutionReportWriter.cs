using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class PipelineExecutionReportWriter
    {
        public void Write(
            PipelineExecutionReportData data,
            string targetFolderPath)
        {
            var itemTimesJson = data.ItemTimesData.ToJson();
            var blockTimesJson = data.BlockTimesData.ToJson();

            var lines = EnumerableUtils.Concat(
                    data.ItemTimesData,
                    new[] { string.Empty }.ToEnumerable(),
                    data.BlockTimesData)
                .Select(x => x.Select(cell => Convert.ToString(cell, CultureInfo.InvariantCulture)).JoinWithSeparator(", "));

            Directory.CreateDirectory(targetFolderPath);

            var csvReportFilePath = Path.Combine(targetFolderPath, "report.csv");
            File.WriteAllLines(csvReportFilePath, lines);
        }
    }
}
