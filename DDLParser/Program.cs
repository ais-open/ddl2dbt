using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DDLParser.TemplateModels;
using DDLParser.Templates;

namespace DDLParser
{
    internal class Program
    {
        private  static Config _config;

        private static void Main(string[] args)
        {
            try
            {
                 _config = ConfigurationProvider.GetConfigSettings();
                //TODO: Remove the argumentDetails Tuple and Create an object for capturing command line arguments.
                var (filePath, fileNames, outputFilePath) = GetCommandlineArgs(args);
                ParseDDL(filePath, fileNames, outputFilePath);
                Console.WriteLine("Files generation completed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in the application: " + e);
            }
        }

        private static (string, string, string) GetCommandlineArgs(string[] args)
        {
            string filepath = "", fileNames = "*", outputFilePath = "";
            if (args.Length > 0)
                for (var i = 0; i < args.Length; i++)
                {
                    if (i == 0)
                    {
                        filepath = args[0];
                        Console.WriteLine(filepath);
                    }

                    if (args[i].Equals("-m"))
                    {
                        fileNames = args[i + 1];
                        Console.WriteLine(fileNames);
                        i += 1;
                    }
                    else if (args[i].Equals("-o"))
                    {
                        outputFilePath = args[i + 1];
                        Console.WriteLine(outputFilePath);
                        i += 1;
                    }
                }
            return (filepath, fileNames, outputFilePath);
        }

        private static void ParseDDL(string filePath, string fileNames, string outputFilePath)
        {
            var rawDdl = @"CREATE TABLE HUB_POLICY 
(
POLICY_HK            BINARY() NOT NULL,
LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
RECORD_SOURCE        VARCHAR(100) NULL,
POLICY_NUMBER        VARCHAR(50) NULL
);
 
ALTER TABLE HUB_POLICY
ADD PRIMARY KEY (POLICY_HK);";


            //rawDdl = File.ReadAllText(@"D:\madhu\GeicoDDLTransformers\docs\Policy Phase 1 v0.13.52 DDL.ddl");

            rawDdl = File.ReadAllText(filePath);

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
        }


        private static void GenerateSatFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Console.WriteLine("generating file for table " + tableName);
                var satTableMetadata = new SatTableMetadata
                {
                    TableName = tableName,
                    SourceModel = "stg_???",
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    SrcHashDiff = Constants.SrcHashDiff,
                    SrcEff = Constants.SrcEff,
                    SrcLdts = Constants.LoadTimestamp,
                    SrcSource = Constants.RecordSource,
                    SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName),
                    SrcPayload = new List<string>()
                };


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

                outputFilePath += "SAT";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var satFileTemplate = new SatFileTemplate(satTableMetadata);
                var content = satFileTemplate.TransformText();
                File.WriteAllText($"{outputFilePath}\\{satTableMetadata.TableName}.sql", content);
                Console.WriteLine($"{outputFilePath}\\{satTableMetadata.TableName}.sql file generated");
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error generating SAT file for {tableName} Exception details: {e}");
            }
        }

        private static void GenerateHubFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
               
                Console.WriteLine("generating file for table " + tableName);
                var hubTableMetadata = new HubTableMetadata
                {
                    TableName = tableName,
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    srcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    srcLdts = Constants.LoadTimestamp,
                    srcSource = Constants.RecordSource,
                    SourceModel = "stg_???",
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

                outputFilePath += "HUB";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var hubFileTemplate = new HubFileTemplate(hubTableMetadata);
                var content = hubFileTemplate.TransformText();
                File.WriteAllText($"{outputFilePath}\\{hubTableMetadata.TableName}.sql", content);
                Console.WriteLine($"{outputFilePath}\\{hubTableMetadata.TableName}.sql file generated");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating HUB file for {tableName} Exception details: {e}");
            }
        }

        private static void GenerateLinkFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Console.WriteLine("generating file for table " + tableName);
              
                var linkTableMetadata = new LinkTableMetadata()
                {
                    TableName = tableName,
                    Columns = DDLHelper.GetDdlStatementColumns(sqlStatement),
                    SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName),
                    SrcLdts = Constants.LoadTimestamp,
                    SrcSource = Constants.RecordSource,
                    SourceModel = "stg_???",
                    SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName)
                };

                outputFilePath += "LNK";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var linkFileTemplate = new LinkFileTemplate(linkTableMetadata);
                var content = linkFileTemplate.TransformText();
                File.WriteAllText($"{outputFilePath}\\{linkTableMetadata.TableName}.sql", content);
                Console.WriteLine($"{outputFilePath}\\{linkTableMetadata.TableName}.sql file generated");
            }

            catch (Exception e)

            {
                Console.WriteLine($"Error generating LNK file for {tableName} Exception details: {e}");
            }
        }
    }
}