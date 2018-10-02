using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Manisero.StreamProcessing.Process.TaskExecutionReporting.ChartsTemplates;
using Manisero.StreamProcessing.Utils;

namespace Manisero.StreamProcessing.Process.TaskExecutionReporting
{
    public class PipelineExecutionReportWriter
    {
        public void Write(
            PipelineExecutionReportData data,
            string targetFolderPath)
        {
            Directory.CreateDirectory(targetFolderPath);

            WriteCsvReport(data, targetFolderPath);
            WriteHtmlReport(data, targetFolderPath);
        }

        private static void WriteCsvReport(
            PipelineExecutionReportData data,
            string targetFolderPath)
        {
            const string reportFileName = "report.csv";

            var lines = EnumerableUtils.Concat(
                    data.ItemTimesData,
                    new[] { string.Empty }.ToEnumerable(),
                    data.BlockTimesData)
                .Select(x => x.Select(cell => Convert.ToString(cell, CultureInfo.InvariantCulture)).JoinWithSeparator(", "));
            
            var csvReportFilePath = Path.Combine(targetFolderPath, reportFileName);
            File.WriteAllLines(csvReportFilePath, lines);
        }

        private static void WriteHtmlReport(
            PipelineExecutionReportData data,
            string targetFolderPath)
        {
            const string itemTimesJsonToken = "@ItemTimesJson";
            const string memoryJsonToken = "@MemoryJson";
            const string blockTimesJsonToken = "@BlockTimesJson";

            var itemTimesJson = data.ItemTimesData.ToJson();
            var memoryJson = data.MemoryData.ToJson();
            var blockTimesJson = data.BlockTimesData.ToJson();

            var chartsTemplatesNamespaceMarker = typeof(ChartsTemplatesNamespaceMarker);
            var chartsTemplateResourceNamePrefix = chartsTemplatesNamespaceMarker.Namespace + ".";
            var chartsTemplatesAssembly = chartsTemplatesNamespaceMarker.Assembly;
            var chartsTemplateResourceNames = chartsTemplatesAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(chartsTemplateResourceNamePrefix));

            foreach (var resourceName in chartsTemplateResourceNames)
            {
                using (var resourceStream = chartsTemplatesAssembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(resourceStream))
                {
                    var template = reader.ReadToEnd();
                    var report = template
                        .Replace(itemTimesJsonToken, itemTimesJson)
                        .Replace(memoryJsonToken, memoryJson)
                        .Replace(blockTimesJsonToken, blockTimesJson);

                    var reportFilePath = Path.Combine(targetFolderPath, resourceName.Substring(chartsTemplateResourceNamePrefix.Length));
                    File.WriteAllText(reportFilePath, report);
                }
            }
        }
    }
}
