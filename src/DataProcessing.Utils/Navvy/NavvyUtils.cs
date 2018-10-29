using System;

namespace DataProcessing.Utils.Navvy
{
    public static class NavvyUtils
    {
        public static string GetTaskTimestampString()
            => DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }
}
