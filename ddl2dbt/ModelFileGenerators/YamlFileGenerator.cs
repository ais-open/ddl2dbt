using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ddl2dbt.Logging;
using ddl2dbt.Parsers;
using ddl2dbt.TemplateModels;
using ddl2dbt.Templates;

namespace ddl2dbt.ModelFileGenerators
{
    internal static class YamlFileGenerator
    {
        public static void GenerateFile(string sqlStatement, string outputFilePath, string tableName, List<CsvDataSource> csvDataSource)
        {
            var yamlFileMetadata = new YamlFileMetadata();
            yamlFileMetadata.TableDefinition = "";
            yamlFileMetadata.TableName = tableName;
            List<CsvDataSource> tableRecords = null;
            try
            {
                if(csvDataSource!=null)
                {
                    tableRecords = csvDataSource.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                var columns = DDLParser.GetDdlStatementColumns(sqlStatement);
                if (tableRecords == null || !tableRecords.Any())
                {
                    List<LabelValuePair> columnsDetails = new List<LabelValuePair>();
                    foreach (var column in columns) 
                    {
                        LabelValuePair columnDetails = new LabelValuePair();
                        columnDetails.Label = column.Name;
                        columnDetails.Value = "";
                        columnsDetails.Add(columnDetails);
                    }
                    yamlFileMetadata.ColumnDefinition = columnsDetails;
                }
                else 
                {
                    yamlFileMetadata.TableDefinition = GetTableDefinition(tableRecords);
                    yamlFileMetadata.ColumnDefinition = GetColumnDefinition(tableRecords, columns);

                }
                yamlFileMetadata.TableDefinition = Regex.Replace(yamlFileMetadata.TableDefinition, @"\s+", " ");
                tableName = tableName.ToLowerInvariant();
                Logger.LogInfo("Generating yml file for " + tableName);
                if (tableName.Contains(Constants.HubFileName, StringComparison.OrdinalIgnoreCase))
                {
                    outputFilePath += "hubs";
                }
                if (tableName.Contains(Constants.LnkFileName, StringComparison.OrdinalIgnoreCase))
                {
                    outputFilePath += "links";
                }
                if (tableName.Contains(Constants.SatFileName, StringComparison.OrdinalIgnoreCase) || tableName.Contains(Constants.MasFileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (tableName.Contains(Constants.SatBrFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        outputFilePath += "satellitebusinessrules";
                    }
                    else
                    {
                        outputFilePath += "satellites";
                    }
                }

                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);

                var yamlFileTemplate = new YamlFileTemplate(yamlFileMetadata);
                var content = yamlFileTemplate.TransformText();

                var pathStr = $"{outputFilePath}\\{tableName}.yml";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generated yml file for " + tableName);
            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("yml", tableName, e.Message));
            }
        }

        private static string GetTableDefinition(List<CsvDataSource> tableRecords) 
        {
            var definition = "";
            foreach (var record in tableRecords)
            {
                if (!record.TableDefinition.Equals(""))
                {
                    definition = record.TableDefinition.Replace("\"", string.Empty).Trim();
                    break;
                }
            }
            return definition;
        }

        private static List<LabelValuePair> GetColumnDefinition(List<CsvDataSource> tableRecords, List<ColumnDetail> columns)
        {
            List<LabelValuePair> columnDefinition = new List<LabelValuePair>();
            foreach (var column in columns)
            {
                LabelValuePair columnDetails = new LabelValuePair();
                columnDetails.Label = column.Name;
                columnDetails.Value = "";
                List<CsvDataSource> columnRecords = tableRecords.Where(e => e.ColumnName.Equals(column.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var record in columnRecords)
                {
                    if (!record.ColumnDefinition.Equals(""))
                    {
                        columnDetails.Value = record.ColumnDefinition.Replace("\"", string.Empty).Trim();
                        columnDetails.Value = Regex.Replace(columnDetails.Value, @"\s+", " ");
                        break;
                    }
                }
                columnDefinition.Add(columnDetails);
            }
            return columnDefinition;

        }

    }
}
