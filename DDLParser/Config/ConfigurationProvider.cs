using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DDLParser
{
    class ConfigurationProvider
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
                var hubFileGenerationSettings = configurationRoot.GetSection($"{configKey}:HubFileGenerationSetting").Get<HubFileGenerationSetting[]>();
                config.HubFileGenerationSettings = hubFileGenerationSettings == null ? Array.Empty<HubFileGenerationSetting>() : hubFileGenerationSettings;

                var lnkFileGenerationSettings = configurationRoot.GetSection($"{configKey}:LnkFileGenerationSetting").Get<LnkFileGenerationSetting[]>();
                config.LnkFileGenerationSettings = lnkFileGenerationSettings == null ? Array.Empty<LnkFileGenerationSetting>() : lnkFileGenerationSettings;

                var satFileGenerationSettings = configurationRoot.GetSection($"{configKey}:SatFileGenerationSetting").Get<SatFileGenerationSetting[]>();
                config.SatFileGenerationSettings = satFileGenerationSettings == null ? Array.Empty<SatFileGenerationSetting>() : satFileGenerationSettings;
            }
            else
            {
               throw new Exception("Application cannot retrieve the configuration information");
            }

            return config;
        }




    }
}
