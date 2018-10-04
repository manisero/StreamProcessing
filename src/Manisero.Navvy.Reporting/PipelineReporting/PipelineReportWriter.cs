using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manisero.Navvy.Reporting.PipelineReporting.Templates;
using Manisero.Navvy.Reporting.Utils;

namespace Manisero.Navvy.Reporting.PipelineReporting
{
    internal interface IPipelineReportWriter
    {
        void Write(
            PipelineReportData data,
            string targetFolderPath);
    }

    internal class PipelineReportWriter : IPipelineReportWriter
    {
        private struct ReportTemplate
        {
            public string FileName { get; set; }
            public string Body { get; set; }
        }

        public const string ReportDataJsonToken = "@ReportDataJson";

        private static readonly Lazy<ICollection<ReportTemplate>> ReportTemplates =
            new Lazy<ICollection<ReportTemplate>>(() => GetReportTemplates().ToArray());

        public void Write(
            PipelineReportData data,
            string targetFolderPath)
        {
            var reportDataJson = data.ToJson();

            foreach (var template in ReportTemplates.Value)
            {
                var report = template.Body.Replace(ReportDataJsonToken, reportDataJson);
                var reportFilePath = Path.Combine(targetFolderPath, $"{data.PipelineName}_{template.FileName}");

                File.WriteAllText(reportFilePath, report);
            }
        }

        private static IEnumerable<ReportTemplate> GetReportTemplates()
        {
            var templatesAssembly = typeof(TemplatesNamespaceMarker).Assembly;
            var templateResourceNamePrefix = typeof(TemplatesNamespaceMarker).Namespace + ".";

            var templateResourceNames = templatesAssembly
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(templateResourceNamePrefix));

            foreach (var resourceName in templateResourceNames)
            {
                using (var resourceStream = templatesAssembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(resourceStream))
                {
                    yield return new ReportTemplate
                    {
                        FileName = resourceName.Substring(templateResourceNamePrefix.Length),
                        Body = reader.ReadToEnd()
                    };
                }
            }
        }
    }
}
