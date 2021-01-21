using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using DDLParser.TemplateModels;
using DDLParser.Templates;
using DDLParser.Templates.StgTemplates;

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
                var (ddlFilePath, csvFilePath, fileNames, outputFilePath) = GetCommandlineArgs(args);
                ParseDDL(ddlFilePath, csvFilePath, fileNames, outputFilePath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Files generation completed.");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured in the application: " + e);
                Console.ResetColor();
            }
        }

        
        private static (string, string, string, string) GetCommandlineArgs(string[] args)
        {
            string ddLfilepath = "", fileNames = "*", outputFilePath = "", csvfilepath = "";
            if (args.Length > 0)
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-ddl"))
                    {
                        ddLfilepath = args[i + 1];
                        Console.WriteLine(ddLfilepath);
                        i += 1; ;
                    }
                    if (args[i].Equals("-csv"))
                    {
                        csvfilepath = args[i + 1];
                        Console.WriteLine(csvfilepath);
                        i += 1;
                    }
                    if (args[i].Equals("-m"))
                    {
                        fileNames = args[i + 1];
                        Console.WriteLine(fileNames);
                        i += 1;
                    }
                    if (args[i].Equals("-o"))
                    {
                        outputFilePath = args[i + 1];
                        Console.WriteLine(outputFilePath);
                        i += 1;
                    }
                }


            //ParseDDL(filepath, fileNames, outputFilePath);
            return (ddLfilepath, csvfilepath, fileNames, outputFilePath);
        }
        private static void ParseDDL(string ddlFilePath, string csvFilePath, string fileNames, string outputFilePath)
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


            //Get Current PROJECT Directory
            var currentProjectDirectoryPath= Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            rawDdl = File.ReadAllText(Path.Combine(currentProjectDirectoryPath, @"docs\\", "Policy Phase 1 v0.13.52 DDL.ddl"));
            csvFilePath = currentProjectDirectoryPath+"\\docs\\Data Source Mapping v0.14.54.csv";
            
            //rawDdl = File.ReadAllText(ddlFilePath);

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
                Console.WriteLine("generating file for table " + tableName);
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
                File.WriteAllText($"{outputFilePath}\\{satTableMetadata.TableName}.sql", content);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{outputFilePath}\\{satTableMetadata.TableName}.sql file generated");
                Console.ResetColor();
            }

            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error generating SAT file for {tableName} Exception details: {e}");
                Console.ResetColor();
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
                File.WriteAllText($"{outputFilePath}\\{hubTableMetadata.TableName}.sql", content);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{outputFilePath}\\{hubTableMetadata.TableName}.sql file generated");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error generating HUB file for {tableName} Exception details: {e}");
                Console.ResetColor();
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
                    SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName)
                };

                var pFrom = tableName.IndexOf('_', +1);
                linkTableMetadata.SourceModel = "stg_lnk" + tableName.Substring(pFrom);
                outputFilePath += "LNK";
                if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                var linkFileTemplate = new LinkFileTemplate(linkTableMetadata);
                var content = linkFileTemplate.TransformText();
                File.WriteAllText($"{outputFilePath}\\{linkTableMetadata.TableName}.sql", content);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{outputFilePath}\\{linkTableMetadata.TableName}.sql file generated");
                Console.ResetColor();
            }

            catch (Exception e)

            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error generating LNK file for {tableName} Exception details: {e}");
                Console.ResetColor();
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