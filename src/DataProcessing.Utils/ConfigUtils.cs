using System;
using DataProcessing.Utils.Settings;
using Microsoft.Extensions.Configuration;

namespace DataProcessing.Utils
{
    public static class ConfigUtils
    {
        private static readonly Lazy<IConfigurationRoot> Config = new Lazy<IConfigurationRoot>(
            () => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        public static IConfigurationRoot GetConfig()
            => Config.Value;

        public static string GetConnectionString(
            this IConfigurationRoot config)
            => config.GetConnectionString(AppDomainUtils.GetCurrentAppName());

        public static AppSettings GetAppSettings()
        {
            var config = GetConfig();

            var appSettings = new AppSettings();
            config.Bind(appSettings);
            appSettings.ConnectionString = config.GetConnectionString();

            return appSettings;
        }
    }
}
