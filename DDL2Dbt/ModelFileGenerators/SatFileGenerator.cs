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
                satTableMetadata.SrcFk = DDLParser.GetForeignKeys(sqlStatements, tableName);
                satTableMetadata.SrcPayload = new List<string>();



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

                satTableMetadata.SourceModel = "stg_" + tableName;
                outputFilePath += "satellites";
                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var satFileTemplate = new SatFileTemplate(satTableMetadata);
                var content = satFileTemplate.TransformText();

                if (satTableMetadata.TableName.StartsWith(Constants.SatBrFileName, StringComparison.OrdinalIgnoreCase))
                    outputFilePath = outputFilePath.Remove(outputFilePath.Length - 1, 1) + "businessrules";


                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var pathStr = $"{outputFilePath}\\{satTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generating sat file for " + tableName);

            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("SAT", tableName, e.Message), "{@SatTableMetadata}", satTableMetadata);
            }
            return satTableMetadata;
        }
    }
}