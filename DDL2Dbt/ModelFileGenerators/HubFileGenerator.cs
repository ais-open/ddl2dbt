using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DDL2Dbt.Logging;
using DDL2Dbt.Parsers;
using DDL2Dbt.TemplateModels;
using DDL2Dbt.Templates;

namespace DDL2Dbt.ModelFileGenerators
{
    internal class HubFileGenerator
    {
        public static HubTableMetadata GenerateFile(string sqlStatement, List<string> sqlStatements,
            string outputFilePath, List<CsvDataSource> records)
        {
            var tableName = DDLParser.GetCreateDdlStatementTableName(sqlStatement);
            tableName = tableName.ToLowerInvariant();
            var hubTableMetadata = new HubTableMetadata();
            try
            {
                Logger.LogInfo("Generating hub file for " + tableName);
                hubTableMetadata.TableName = tableName;
                hubTableMetadata.Columns = DDLParser.GetDdlStatementColumns(sqlStatement);
                hubTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName);
                hubTableMetadata.SrcPk = hubTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                hubTableMetadata.SrcLdts = Constants.LoadTimestamp;
                hubTableMetadata.SrcSource = Constants.RecordSource;
                hubTableMetadata.SrcNk = new List<string>();
                hubTableMetadata.Tags = CsvParser.GetTags(records, tableName);


                foreach (var column in hubTableMetadata.Columns)
                    if (
                        hubTableMetadata.PrimaryKeys.Any(s => s.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(column.Name, hubTableMetadata.SrcLdts, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(column.Name, hubTableMetadata.SrcSource, StringComparison.OrdinalIgnoreCase))
                    {
                    }
                    else
                    {
                        hubTableMetadata.SrcNk.Add(column.Name);
                    }

               

                hubTableMetadata.SourceModel = "stg_" + tableName;
                outputFilePath += "hubs";
                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var hubFileTemplate = new HubFileTemplate(hubTableMetadata);
                var content = hubFileTemplate.TransformText();

                var pathStr = $"{outputFilePath}\\{hubTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generated hub file for " + tableName);

            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("HUB", tableName, e.Message), "{@HubTableMetadata}", hubTableMetadata);
            }
            return hubTableMetadata;
        }
    }
}