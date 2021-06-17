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
                satTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName, records);
                if (satTableMetadata.PrimaryKeys.Contains(Constants.NotFoundString) && satTableMetadata.PrimaryKeys.Count == 1)
                {
                    satTableMetadata.SrcPk = Constants.NotFoundString;
                }
                else
                {
                    satTableMetadata.SrcPk = satTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                }
                satTableMetadata.SrcHashDiff = Constants.SrcHashDiff;
                satTableMetadata.SrcEff = Constants.SrcEff;
                satTableMetadata.SrcLdts = Constants.LoadTimestamp;
                satTableMetadata.SrcSource = Constants.RecordSource;
                satTableMetadata.SrcPayload = new List<string>();
                satTableMetadata.IsFIleTypeMAS = false;
                satTableMetadata.CompositeKeysPresent = false;
                
                satTableMetadata.SrcCdk = new List<string>();
                satTableMetadata.MaskedColumns = new List<LabelValuePair>();
                satTableMetadata.MaskedColumnsPresent = false;

                List<CsvDataSource> tableRecords = null;
                if (records != null)
                {
                    tableRecords = records.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
                    foreach (var record in tableRecords)
                    {
                        if ( !string.IsNullOrWhiteSpace(record.SpiClassification)  && !record.SpiClassification.Equals("Confidential", StringComparison.OrdinalIgnoreCase))
                        {
                            satTableMetadata.MaskedColumnsPresent = true;
                            break;
                        }
                    }
                    if (satTableMetadata.MaskedColumnsPresent)
                    {
                        var MaskedRecords = tableRecords.Where(e => !e.SpiClassification.Equals("Confidential", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(e.SpiClassification)).ToList();
                        foreach (var Maskedrecord in MaskedRecords)
                        {
                            var MaskedRecord = new LabelValuePair { Label = Maskedrecord.ColumnName, Value = Maskedrecord.SpiClassification};
                            satTableMetadata.MaskedColumns.Add(MaskedRecord);
                        }
                    }
                }
                
                if (satTableMetadata.PrimaryKeys.Count() > 1)
                {
                    satTableMetadata.CompositeKeysPresent = true;
                    satTableMetadata.Compositekeys = satTableMetadata.PrimaryKeys.Where(e => !e.Contains("_HK", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                satTableMetadata.Tags = CsvParser.GetTags(records, tableName);

                if (tableName.Contains("MAS", StringComparison.OrdinalIgnoreCase))
                {
                    satTableMetadata.SourceModel = new List<string> { Constants.NotFoundString };
                    satTableMetadata.IsFIleTypeMAS = true;
                }
                else
                {
                    satTableMetadata.SourceModel = CsvParser.GetSourceModel(records, tableName);
                }

                if (satTableMetadata.IsFIleTypeMAS) 
                {
                    satTableMetadata.SrcCdk = satTableMetadata.Compositekeys;
                    if (satTableMetadata.SrcCdk == null || !satTableMetadata.SrcCdk.Any())
                    {
                        satTableMetadata.SrcCdk = new List<string> { Constants.NotFoundString };
                    }
                    //if (satTableMetadata.SrcCdk.Contains(Constants.LoadTimestamp)) 
                    //{
                    //    satTableMetadata.SrcCdk.Remove(Constants.LoadTimestamp);
                    //}
                }

                foreach (var column in satTableMetadata.Columns)
                    if (
                        string.Equals(column.Name, satTableMetadata.SrcPk, StringComparison.OrdinalIgnoreCase) ||
                        satTableMetadata.SrcCdk.Any(s => s.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) ||
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