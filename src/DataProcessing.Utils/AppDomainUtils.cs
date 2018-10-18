using System;

namespace DataProcessing.Utils
{
    public static class AppDomainUtils
    {
        public static string GetCurrentAppName()
            => AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
    }
}
