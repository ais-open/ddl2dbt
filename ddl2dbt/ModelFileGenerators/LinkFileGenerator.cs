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
    internal class LinkFileGenerator
    {
        public static LinkTableMetadata GenerateFile(string sqlStatement, List<string> sqlStatements,
            string outputFilePath,  List<CsvDataSource> records)
        {
            var tableName = DDLParser.GetCreateDdlStatementTableName(sqlStatement);
            tableName = tableName.ToLowerInvariant();
            var linkTableMetadata = new LinkTableMetadata();
            try
            {
                Logger.LogInfo("Generating link file for " + tableName);
                linkTableMetadata.TableName = tableName;
                linkTableMetadata.Columns = DDLParser.GetDdlStatementColumns(sqlStatement);
                linkTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName, records);
                if (linkTableMetadata.PrimaryKeys.Contains(Constants.NotFoundString) && linkTableMetadata.PrimaryKeys.Count == 1)
                {
                    linkTableMetadata.SrcPk = Constants.NotFoundString;
                }
                else
                {
                    linkTableMetadata.SrcPk = linkTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                }
                linkTableMetadata.SrcLdts = Constants.LoadTimestamp;
                linkTableMetadata.SrcSource = Constants.RecordSource;
                linkTableMetadata.SrcFk = DDLParser.GetForeignKeys(sqlStatements, tableName, records);
                linkTableMetadata.Tags = CsvParser.GetTags(records, tableName);

                linkTableMetadata.MaskedColumns = new List<LabelValuePair>();
                linkTableMetadata.MaskedColumnsPresent = false;

                List<CsvDataSource> tableRecords = null;
                if (records != null)
                {
                    tableRecords = records.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
                    foreach (var record in tableRecords)
                    {
                        if (!string.IsNullOrWhiteSpace(record.SpiClassification) && !record.SpiClassification.Equals("Confidential", StringComparison.OrdinalIgnoreCase))
                        {
                            linkTableMetadata.MaskedColumnsPresent = true;
                            break;
                        }
                    }
                    if (linkTableMetadata.MaskedColumnsPresent)
                    {
                        var MaskedRecords = tableRecords.Where(e => !e.SpiClassification.Equals("Confidential", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(e.SpiClassification)).ToList();
                        foreach (var Maskedrecord in MaskedRecords)
                        {
                            var MaskedRecord = new LabelValuePair { Label = Maskedrecord.ColumnName, Value = Maskedrecord.SpiClassification };
                            linkTableMetadata.MaskedColumns.Add(MaskedRecord);
                        }
                    }
                }

                linkTableMetadata.SourceModel = CsvParser.GetSourceModel(records, tableName);
                outputFilePath += "links";
                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                var linkFileTemplate = new LinkFileTemplate(linkTableMetadata);
                var content = linkFileTemplate.TransformText();

                var pathStr = $"{outputFilePath}\\{linkTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Logger.LogInfo("Generated link file for " + tableName);
            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("LNK", tableName, e.Message), "{@LinkTableMetadata}", linkTableMetadata);
            }
            return linkTableMetadata;
        }
    }
}