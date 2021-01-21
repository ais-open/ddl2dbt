using System;
using Microsoft.Extensions.Configuration;

namespace DDLParser
{
    class ConfigurationProvider
    {
        public static Config GetConfigSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            var configurationRoot = configurationBuilder.Build();

            const string configKey = "Config";
            var config = configurationRoot.GetSection(configKey).Get<Config>();

            if (config != null)
            {
                var hubFileGenerationSettings = configurationRoot.GetSection($"{configKey}:HubFileGenerationSetting").Get<HubFileGenerationSetting[]>();
                config.HubFileGenerationSettings = hubFileGenerationSettings == null ? Array.Empty<HubFileGenerationSetting>() : hubFileGenerationSettings;
            }
            else
            {
               throw new Exception("Application cannot retrieve the configuration information");
            }

            return config;
        }




    }
}
