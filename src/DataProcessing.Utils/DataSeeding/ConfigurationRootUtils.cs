using Microsoft.Extensions.Configuration;

namespace DataProcessing.Utils.DataSeeding
{
    public static class ConfigurationRootUtils
    {
        public static DataSetup GetDataSetup(
            this IConfigurationRoot config)
            => config.GetSection(nameof(DataSetup)).BindAndReturn(new DataSetup());
    }
}
