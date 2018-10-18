using System;
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

        public static TTarget BindAndReturn<TTarget>(
            this IConfigurationSection section,
            TTarget target)
        {
            section.Bind(target);
            return target;
        }
    }
}
