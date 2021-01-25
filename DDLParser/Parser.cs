using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using DDL2Dbt.Config;
using DDL2Dbt.TemplateModels;
using DDL2Dbt.Templates;
using DDL2Dbt.Templates.StgTemplates;
using Serilog;

namespace DDL2Dbt
{
    public class Parser
    {
        private static Config.Config _config;
        public static void ParseDDL(string ddlFilePath, string csvFilePath, string fileNames, string outputFilePath)
        {
            _config = ConfigurationProvider.GetConfigSettings();
            var rawDdl = @"CREATE TABLE HUB_POLICY 
(
POLICY_HK            BINARY() NOT NULL,
LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
RECORD_SOURCE        VARCHAR(100) NULL,
POLICY_NUMBER        VARCHAR(50) NULL
);
 
ALTER TABLE HUB_POLICY
ADD PRIMARY KEY (POLICY_HK);";


            //Get Current PROJECT Directory
            // var currentProjectDirectoryPath= Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            //rawDdl = File.ReadAllText(Path.Combine(currentProjectDirectoryPath, @"docs\\", "Policy Phase 1 v0.13.52 DDL.ddl"));
            // csvFilePath = currentProjectDirectoryPath+"\\docs\\Data Source Mapping v0.14.54.csv";

            rawDdl = File.ReadAllText(ddlFilePath);

            var sqlStatements = DDLHelper.BuildDdlStatementsCollection(rawDdl);
            var fileNameArr = fileNames.Split(',');

            foreach (var sqlStatement in sqlStatements)
                if (!string.IsNullOrWhiteSpace(sqlStatement))
                {
                    if (Array.Exists(fileNameArr, element => string.Equals(element, "hub", StringComparison.OrdinalIgnoreCase) ||
                                                             string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                        if (sqlStatement.Contains("CREATE TABLE HUB", StringComparison.OrdinalIgnoreCase))
                            //if (sqlStatement.Contains("CREATE TABLE HUB_POLICY"))
                        {
                            GenerateHubFile(sqlStatement, sqlStatements, outputFilePath);
                        }

                    if (Array.Exists(fileNameArr, element => string.Equals(element, "lnk", StringComparison.OrdinalIgnoreCase) ||
                                                             string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                        if (sqlStatement.Contains("CREATE TABLE LNK", StringComparison.OrdinalIgnoreCase))
                            // if (sqlStatement.Contains("CREATE TABLE LNK_POLICY_INSURES_VEHICLE"))

                        {
                            GenerateLinkFile(sqlStatement, sqlStatements, outputFilePath);
                        }

                    if (Array.Exists(fileNameArr, element => string.Equals(element, "sat", StringComparison.OrdinalIgnoreCase) ||
                                                             string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                        if (sqlStatement.Contains("CREATE TABLE SAT", StringComparison.OrdinalIgnoreCase))
                        {
                            //if (sqlStatement.Contains("CREATE TABLE SAT_PEAK_POLICY"))
                            {
                                GenerateSatFile(sqlStatement, sqlStatements, outputFilePath);
                            }
                        }
                }

            if (!csvFilePath.Equals(""))
                if (Array.Exists(fileNameArr, element => string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) || string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                    GenerateStgFile(ddlFilePath, csvFilePath, outputFilePath);
        }

        private static void GenerateSatFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Log.Information($"Generating file for table {tableName}");
                var satTableMetadata = new SatTableMetadata
                {
                    TableName = tableName,
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    SrcHashDiff = Constants.SrcHashDiff,
                    SrcEff = Constants.SrcEff,
                    SrcLdts = Constants.LoadTimestamp,
                    SrcSource = Constants.RecordSource,
                    SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName),
                    SrcPayload = new List<string>()
                };

                if (_config.SatFileGenerationSettings
                    .SingleOrDefault(e => string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    satTableMetadata.Tags = _config.SatFileGenerationSettings.Single(e =>
                        string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)).Tags;
                }

                foreach (var column in satTableMetadata.Columns)
                    if (
                        //string.Equals(column.Name, satTableMetadata.SrcPk, StringComparison.OrdinalIgnoreCase) ||
                        satTableMetadata.SrcPk.Any(s => s.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) ||
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

                var pFrom = tableName.IndexOf('_', tableName.IndexOf('_') + 1);
                satTableMetadata.SourceModel = "stg_sat" + tableName.Substring(pFrom);

                outputFilePath += "SAT";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var satFileTemplate = new SatFileTemplate(satTableMetadata);
                var content = satFileTemplate.TransformText();
                
                var pathStr = $"{outputFilePath}\\{satTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Log.Information("Generated file  "+pathStr);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error generating SAT file for table {tableName}");
            }
        }

        private static void GenerateHubFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Log.Information($"Generating file for table {tableName}");
                var hubTableMetadata = new HubTableMetadata
                {
                    TableName = tableName,
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    srcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    srcLdts = Constants.LoadTimestamp,
                    srcSource = Constants.RecordSource,

                    srcNk = new List<string>(),
                    //Tags = _config.HubFileGenerationSettings.Single(e =>
                    //    string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)).Tags
                };

                if (_config.HubFileGenerationSettings
                    .SingleOrDefault(e => string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    hubTableMetadata.Tags = _config.HubFileGenerationSettings.Single(e =>
                        string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)).Tags;
                }

                foreach (var column in hubTableMetadata.Columns)
                    if (
                        hubTableMetadata.srcPk.Any(s => s.Equals(column.Name, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(column.Name, hubTableMetadata.srcLdts, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(column.Name, hubTableMetadata.srcSource, StringComparison.OrdinalIgnoreCase))
                    {
                    }
                    else
                    {
                        hubTableMetadata.srcNk.Add(column.Name);
                    }

                var pFrom = tableName.IndexOf('_', +1);
                hubTableMetadata.SourceModel = "stg_hub" + tableName.Substring(pFrom);

                outputFilePath += "HUB";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var hubFileTemplate = new HubFileTemplate(hubTableMetadata);
                var content = hubFileTemplate.TransformText();

                var pathStr = $"{outputFilePath}\\{hubTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Log.Information("Generated file  " + pathStr);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error generating HUB file for table {tableName}");

            }
        }

        private static void GenerateLinkFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Log.Information($"Generating file for table {tableName}");


                var linkTableMetadata = new LinkTableMetadata()
                {
                    TableName = tableName,
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    SrcLdts = Constants.LoadTimestamp,
                    SrcSource = Constants.RecordSource,
                    SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName)
                };

                if (_config.LnkFileGenerationSettings
                    .SingleOrDefault(e => string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    linkTableMetadata.Tags =_config.LnkFileGenerationSettings.Single(e =>
                        string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)).Tags;
                }

                var pFrom = tableName.IndexOf('_', +1);
                linkTableMetadata.SourceModel = "stg_lnk" + tableName.Substring(pFrom);
                outputFilePath += "LNK";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var linkFileTemplate = new LinkFileTemplate(linkTableMetadata);
                var content = linkFileTemplate.TransformText();
               
                var pathStr = $"{outputFilePath}\\{linkTableMetadata.TableName}.sql";
                File.WriteAllText(pathStr, content);
                Log.Information("Generated file  " + pathStr);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error generating LNK file for table {tableName}");
            }
        }

        private static void GenerateStgFile(string rawDDLPath, string csvFilePath, string outputFilePath)
        {
            //GetCsvData(csvFilePath);
            var tables = new List<string>();

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = csv.GetRecord<DataSource>();
                    if (!tables.Contains(record.TableName))
                    {
                        tables.Add(record.TableName);
                    }
                }
            }
            foreach (var name in tables)
            {
                switch (name)
                {
                    case "SAT_PEAK_POLICY":
                        var stgMetadata = new StgMetadata();
                        using (var reader = new StreamReader(csvFilePath))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            csv.Read();
                            csv.ReadHeader();
                            while (csv.Read())
                            {
                                var record = csv.GetRecord<DataSource>();
                                if ((record.TableName).Equals(name))
                                {
                                    if (!record.DataSourceTableName.Equals(""))
                                    {
                                        stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                        stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                        break;
                                    }
                                }
                            }
                            var satPeakPolicyTemplate = new SatPeakPolicyTemplate(stgMetadata);
                            var content = satPeakPolicyTemplate.TransformText();
                            File.WriteAllText(name + $".sql", content);
                        }

                        break;
                    default:
                        // code block
                        break;
                }
            }

        }
    }
}