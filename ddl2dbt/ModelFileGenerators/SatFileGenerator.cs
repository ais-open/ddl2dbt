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
    internal class SatFileGenerator
    {
        public static SatTableMetadata GenerateFile(string sqlStatement, List<string> sqlStatements,
            string outputFilePath, List<CsvDataSource> records)
        {
            var tableName = DDLParser.GetCreateDdlStatementTableName(sqlStatement);
            tableName = tableName.ToLowerInvariant();

            var satTableMetadata = new SatTableMetadata();
            try
            {
                Logger.LogInfo("Generating sat file for " + tableName);
                satTableMetadata.TableName = tableName;
                satTableMetadata.Columns = DDLParser.GetDdlStatementColumns(sqlStatement);
                satTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName);
                satTableMetadata.SrcPk = satTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                satTableMetadata.SrcHashDiff = Constants.SrcHashDiff;
                satTableMetadata.SrcEff = Constants.SrcEff;
                satTableMetadata.SrcLdts = Constants.LoadTimestamp;
                satTableMetadata.SrcSource = Constants.RecordSource;
                satTableMetadata.SrcPayload = new List<string>();
                satTableMetadata.IsFIleTypeMAS = false;
                satTableMetadata.CompositeKeysPresent = false;

                if (satTableMetadata.PrimaryKeys.Count() > 1)
                {
                    satTableMetadata.CompositeKeysPresent = true;
                    satTableMetadata.Compositekeys = satTableMetadata.PrimaryKeys.Where(e => !e.Contains("_HK", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                satTableMetadata.Tags = CsvParser.GetTags(records, tableName);

                foreach (var column in satTableMetadata.Columns)
                    if (
                        string.Equals(column.Name, satTableMetadata.SrcPk, StringComparison.OrdinalIgnoreCase) ||
                        //satTableMetadata.PrimaryKeys.Any(s => s.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(column.Name, satTableMetadata.SrcHashDiff, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(column.Name, satTableMetadata.SrcEff, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(column.Name, satTableMetadata.SrcLdts, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(column.Name, satTableMetadata.SrcSource, StringComparison.OrdinalIgnoreCase))
                    {
                    }
                    else
                    {
                        satTableMetadata.SrcPayload.Add(column.Name);
                    }
                if (tableName.Contains("MAS", StringComparison.OrdinalIgnoreCase))
                {
                    satTableMetadata.SourceModel = new List<string> { Constants.NotFoundString };
                    satTableMetadata.IsFIleTypeMAS = true;
                }
                else 
                {
                    satTableMetadata.SourceModel = CsvParser.GetSourceModel(records, tableName);
                }
                outputFilePath += "satellites";
                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var satFileTemplate = new SatFileTemplate(satTableMetadata);
                var content = satFileTemplate.TransformText();

                if (satTableMetadata.TableName.StartsWith(Constants.SatBrFileName, StringComparison.OrdinalIgnoreCase))
                    outputFilePath = outputFilePath.Remove(outputFilePath.Length - 1, 1) + "businessrules";


                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var pathStr = $"{outputFilePath}\\{satTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generated sat file for " + tableName);

            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("SAT", tableName, e.Message), "{@SatTableMetadata}", satTableMetadata);
            }
            return satTableMetadata;
        }
    }
}