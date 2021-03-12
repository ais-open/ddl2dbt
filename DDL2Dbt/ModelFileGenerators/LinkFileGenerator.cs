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
                linkTableMetadata.PrimaryKeys = DDLParser.GetPrimaryKey(sqlStatements, tableName);
                linkTableMetadata.SrcPk = linkTableMetadata.PrimaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                linkTableMetadata.SrcLdts = Constants.LoadTimestamp;
                linkTableMetadata.SrcSource = Constants.RecordSource;
                linkTableMetadata.SrcFk = DDLParser.GetForeignKeys(sqlStatements, tableName);
                linkTableMetadata.Tags = CsvParser.GetTags(records, tableName);

                linkTableMetadata.SourceModel = "stg_" + tableName;
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