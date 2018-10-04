using System;

namespace Manisero.Navvy.Reporting.Utils
{
    internal static class ReportingUtils
    {
        public static object GetLogValue(this TimeSpan timeSpan) => timeSpan.TotalMilliseconds;

        public static double ToMb(this long bytes) => bytes / 1024d / 1024d;
    }
}
