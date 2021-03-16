using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ddl2dbt.Config
{
    internal class ConfigurationProvider
    {
        public static Config GetConfigSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json");
            var configurationRoot = configurationBuilder.Build();

            const string configKey = "Config";
            var config = configurationRoot.GetSection(configKey).Get<Config>();

            if (config != null)
            {
                var csvFileSettings = configurationRoot.GetSection($"{configKey}:CSVFileSettings").Get<CSVFileSettings[]>();
                config.CSVFileSettings = csvFileSettings ?? Array.Empty<CSVFileSettings>();

            }
            else
            {
                throw new Exception("Application cannot retrieve the configuration information");
            }

            return config;
        }




    }
}
