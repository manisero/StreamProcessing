using System;
using DataProcessing.Utils.Settings;
using Microsoft.Extensions.Configuration;

namespace DataProcessing.Utils
{
    public static class ConfigUtils
    {
        private static readonly Lazy<IConfigurationRoot> Config = new Lazy<IConfigurationRoot>(
            () => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        public static AppSettings GetAppSettings()
        {
            var config = Config.Value;

            var appSettings = new AppSettings();
            config.Bind(appSettings);

            var connectionStringName = config.GetSection(AppSettings.ConnectionStringNamesSectionName)[AppDomainUtils.GetCurrentAppName()];
            appSettings.ConnectionString = config.GetConnectionString(connectionStringName);

            return appSettings;
        }
    }
}
