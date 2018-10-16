using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace StreamProcessing.Utils
{
    public static class ConfigUtils
    {
        private static readonly Lazy<IConfigurationRoot> Config = new Lazy<IConfigurationRoot>(
            () => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        public static IConfigurationRoot GetConfig()
            => Config.Value;

        public static string GetDefaultConnectionString(
            this IConfigurationRoot config)
            => config.GetConnectionString(Assembly.GetEntryAssembly().FullName.Split('.')[0]);
    }
}
