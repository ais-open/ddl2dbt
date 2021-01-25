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
            var currentProjectDirectoryPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            //rawDdl = File.ReadAllText(ddlFilePath);
            rawDdl = File.ReadAllText(Path.Combine(currentProjectDirectoryPath, @"docs\\", "Policy Phase 1 v0.13.52 DDL.ddl"));
            csvFilePath = currentProjectDirectoryPath + "\\docs\\Data Source Mapping v0.14.54.csv";
            Console.WriteLine(csvFilePath);

            IEnumerable<DataSource> records = null;
            if (!csvFilePath.Equals(""))
            {
                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<DataSource>().ToList();
                }
            }

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
                            var hubTableMetadata = GenerateHubFile(sqlStatement, sqlStatements, outputFilePath);
                            if (!csvFilePath.Equals(""))
                            {
                                if (Array.Exists(fileNameArr, element => string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) || string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                                    GenerateStgFile(hubTableMetadata.TableName, hubTableMetadata.SourceModel, records, outputFilePath);
                            }
                        }

                    if (Array.Exists(fileNameArr, element => string.Equals(element, "lnk", StringComparison.OrdinalIgnoreCase) ||
                                                             string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                        if (sqlStatement.Contains("CREATE TABLE LNK", StringComparison.OrdinalIgnoreCase))
                            // if (sqlStatement.Contains("CREATE TABLE LNK_POLICY_INSURES_VEHICLE"))

                        {
                            var linkTableMetadata = GenerateLinkFile(sqlStatement, sqlStatements, outputFilePath);
                            if (!csvFilePath.Equals(""))
                            {
                                if (Array.Exists(fileNameArr, element => string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) || string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                                    GenerateStgFile(linkTableMetadata.TableName, linkTableMetadata.SourceModel, records, outputFilePath);
                            }
                        }

                    if (Array.Exists(fileNameArr, element => string.Equals(element, "sat", StringComparison.OrdinalIgnoreCase) ||
                                                             string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                        if (sqlStatement.Contains("CREATE TABLE SAT", StringComparison.OrdinalIgnoreCase))
                        {
                            //if (sqlStatement.Contains("CREATE TABLE SAT_PEAK_POLICY"))
                            {
                                var satTableMetadata = GenerateSatFile(sqlStatement, sqlStatements, outputFilePath);
                                if (!csvFilePath.Equals(""))
                                {
                                    if (Array.Exists(fileNameArr, element => string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) || string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                                        GenerateStgFile(satTableMetadata.TableName, satTableMetadata.SourceModel, records, outputFilePath);
                                }
                            }
                        }
                }
        }

        private static SatTableMetadata GenerateSatFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            var satTableMetadata = new SatTableMetadata();
            try
            {
                Log.Information($"Generating file for table {tableName}");
                satTableMetadata.TableName = tableName;
                satTableMetadata.Columns = DDLHelper.GetDdlStatementColumns(sqlStatement);
                satTableMetadata.SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName);
                satTableMetadata.SrcHashDiff = Constants.SrcHashDiff;
                satTableMetadata.SrcEff = Constants.SrcEff;
                satTableMetadata.SrcLdts = Constants.LoadTimestamp;
                satTableMetadata.SrcSource = Constants.RecordSource;
                satTableMetadata.SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName);
                satTableMetadata.SrcPayload = new List<string>();

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
            return satTableMetadata;
        }

        private static HubTableMetadata GenerateHubFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            var hubTableMetadata = new HubTableMetadata();
            try
            {
                Log.Information($"Generating file for table {tableName}");
                hubTableMetadata.TableName = tableName;
                hubTableMetadata.Columns = DDLHelper.GetDdlStatementColumns(sqlStatement);
                hubTableMetadata.srcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName);
                hubTableMetadata.srcLdts = Constants.LoadTimestamp;
                hubTableMetadata.srcSource = Constants.RecordSource;
                hubTableMetadata.srcNk = new List<string>();

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
            return hubTableMetadata;
        }

        private static LinkTableMetadata GenerateLinkFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = DDLHelper.GetCreateDdlStatementTableName(sqlStatement);
            var linkTableMetadata = new LinkTableMetadata();
            try
            {
                Log.Information($"Generating file for table {tableName}");


                linkTableMetadata.TableName = tableName;
                linkTableMetadata.Columns = DDLHelper.GetDdlStatementColumns(sqlStatement);
                linkTableMetadata.SrcPk = DDLHelper.GetPrimaryKey(sqlStatements, tableName);
                linkTableMetadata.SrcLdts = Constants.LoadTimestamp;
                linkTableMetadata.SrcSource = Constants.RecordSource;
                linkTableMetadata.SrcFk = DDLHelper.GetForeignKeys(sqlStatements, tableName);

                if (_config.LnkFileGenerationSettings
                    .SingleOrDefault(e => string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    linkTableMetadata.Tags =_config.LnkFileGenerationSettings.Single(e =>
                        string.Equals(e.TableName, tableName, StringComparison.OrdinalIgnoreCase)).Tags;
                }

                else
                {
                    Log.Warning($"Could not find tags for table {tableName} in the configuration");
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
            return linkTableMetadata;
        }

        private static void GenerateStgFile(string tableName, string sourceModel, IEnumerable<DataSource> csvDataSource, string outputFilePath)
        {
            var stgMetadata = new StgMetadata();
            stgMetadata.DataSourceTableName = "???";
            stgMetadata.DataSourceObjectSystem = "???";
            switch (tableName)
            {
                case "SAT_PEAK_POLICY":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var satPeakPolicyTemplate = new SatPeakPolicyTemplate(stgMetadata);
                    var content = satPeakPolicyTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);

                    break;

                case "LNK_POLICY_HAS_VEHICLE_COVERAGE":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var lnkPolicyHasVehicleCoverageTemplate = new LnkPolicyHasVehicleCoverageTemplate(stgMetadata);
                    content = lnkPolicyHasVehicleCoverageTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;


                case "LNK_POLICY_HAS_TRANSACTION":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var lnkPolicyHasTrancactionTemplate = new LnkPolicyHasTrancactionTemplate(stgMetadata);
                    content = lnkPolicyHasTrancactionTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;

                case "LNK_POLICY_INSURES_VEHICLE":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var lnkPolicyInsuresVehicleTemplate = new LnkPolicyInsuresVehicleTemplate(stgMetadata);
                    content = lnkPolicyInsuresVehicleTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;

                case "SAT_PEAK_VEHICLE":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var SatPeakVehicleTemplate = new SatPeakVehicleTemplate(stgMetadata);
                    content = SatPeakVehicleTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;

                case "SAT_PEAK_TRANSACTION":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var satPeakTransactionTemplate = new SatPeakTransactionTemplate(stgMetadata);
                    content = satPeakTransactionTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;

                case "SAT_BR_COVERAGE_REF_COV_DED_LIT":
                case "SAT_BR_COVERAGE_REF_COV_LIMIT_LIT":
                case "SAT_BR_COVERAGE_REF_COV_MNEMONICS":
                case "SAT_BR_POLICY_RATING_STRUCTURE":
                case "SAT_BR_POLICY_REF_PRODCT_RSK_TYP_TBL":
                case "SAT_BR_POLICY_RISK_SEGMENT":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }
                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var satBrCoverageRefCovDedLitTemplate = new SatBrCoverageRefCovDedLitTemplate(stgMetadata);
                    content = satBrCoverageRefCovDedLitTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);


                    break;

                case "SAT_PEAK_VEHICLE_VINSYMBOL":
                case "SAT_PEAK_VEHICLE_REGISTRATIONOWNERSHIP":
                case "SAT_PEAK_VEHICLE_USAGEDETAILS":

                    foreach (var record in csvDataSource)
                    {
                        if ((record.TableName).Equals(tableName))
                        {
                            if (!record.DataSourceTableName.Equals(""))
                            {
                                stgMetadata.DataSourceTableName = record.DataSourceTableName;
                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;
                                break;
                            }
                        }
                    }

                    outputFilePath += "STG";
                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);
                    var satPeakVehicleVinsymbolTemplate = new SatPeakVehicleVinsymbolTemplate(stgMetadata);
                    content = satPeakVehicleVinsymbolTemplate.TransformText();
                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);

                    break;


                case "SAT_RDS_COV_MNEMONICS":

                case "SAT_RDS_COV_DED_LIT":

                case "SAT_RDS_COV_LIMIT_LIT":

                case "SAT_RDS_PRODCT_RSK_TYP_TBL":

                case "SAT_PEAK_VEHICLE_PII":

                case "SAT_PEAK_VEHICLE_VINSYMBOL_PII":



                    foreach (var record in csvDataSource)

                    {

                        if ((record.TableName).Equals(tableName))

                        {

                            if (!record.DataSourceTableName.Equals(""))

                            {

                                stgMetadata.DataSourceTableName = record.DataSourceTableName;

                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;

                                break;

                            }

                        }

                    }

                    outputFilePath += "STG";

                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);

                    var satRdsCovMnemonicsTemplate = new SatRdsCovMnemonicsTemplate(stgMetadata);

                    content = satRdsCovMnemonicsTemplate.TransformText();

                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);



                    break;



                case "SAT_PEAK_RISK_COVERAGE":



                    foreach (var record in csvDataSource)

                    {

                        if ((record.TableName).Equals(tableName))

                        {

                            if (!record.DataSourceTableName.Equals(""))

                            {

                                stgMetadata.DataSourceTableName = record.DataSourceTableName;

                                stgMetadata.DataSourceObjectSystem = record.DataSourceObjectSystem;

                                break;

                            }

                        }

                    }

                    outputFilePath += "STG";

                    if (!Directory.Exists(outputFilePath)) Directory.CreateDirectory(outputFilePath);

                    var satPeakRiskCoverageTemplate = new SatPeakRiskCoverageTemplate(stgMetadata);

                    content = satPeakRiskCoverageTemplate.TransformText();

                    File.WriteAllText($"{outputFilePath}\\{sourceModel}.sql", content);



                    break;

                default:
                    // code block
                    break;

            }

        }
    }
}