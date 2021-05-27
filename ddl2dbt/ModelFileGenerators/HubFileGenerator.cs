using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ddl2dbt.Logging;
using ddl2dbt.Parsers;
using ddl2dbt.TemplateModels;
using ddl2dbt.Templates;

namespace ddl2dbt.ModelFileGenerators
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
                hubTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName, records);
                if (hubTableMetadata.PrimaryKeys.Contains(Constants.NotFoundString) && hubTableMetadata.PrimaryKeys.Count == 1)
                {
                    hubTableMetadata.SrcPk = Constants.NotFoundString;
                }
                else 
                {
                    hubTableMetadata.SrcPk = hubTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                }
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

               

                hubTableMetadata.SourceModel = CsvParser.GetSourceModel(records, tableName);
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