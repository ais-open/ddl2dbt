using System;
using System.IO;
using ddl2dbt.Logging;
using ddl2dbt.Templates;

namespace ddl2dbt.ModelFileGenerators
{
    internal static class DocFileGenerator
    {
        public static void GenerateFile(string outputFilePath, string tableName)
        {
            try
            {
                tableName = tableName.ToLowerInvariant();
                Logger.LogInfo("Generating docs file for " + tableName);
              
                string modelName = Constants.NotFoundString;

                if (tableName.Contains(Constants.HubFileName, StringComparison.OrdinalIgnoreCase))
                {
                    outputFilePath += "docs\\hubs";
                    modelName = "Hub";
                }
                if (tableName.Contains(Constants.LnkFileName, StringComparison.OrdinalIgnoreCase))
                {
                    outputFilePath += "docs\\links";
                    modelName = "Lnk";
                }
                if (tableName.Contains(Constants.SatFileName, StringComparison.OrdinalIgnoreCase) || tableName.Contains(Constants.MasFileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (tableName.Contains(Constants.SatBrFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        outputFilePath += "docs\\satellitebusinessrules";
                        modelName = "Sat";
                    }
                    else
                    {
                        outputFilePath += "docs\\satellites";
                        modelName = "Sat";
                    }
                }

                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);

                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var docFileTemplate = new DocFileTemplate(tableName.ToLowerInvariant(), modelName);
                var content = docFileTemplate.TransformText();

                var pathStr = $"{outputFilePath}\\{tableName}.docs";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generated docs file for " + tableName);
            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("docs", tableName, e.Message));
            }
        }
    }
}
