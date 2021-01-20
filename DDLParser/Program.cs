using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DDLParser.Templates;

namespace DDLParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {



                //TODO: Remove the argumentDetails Tuple and Create an object for capturing command line arguments.

                var argumentDetails = GetCommandlineArgs(args);
                ParseDDL(argumentDetails.Item1, argumentDetails.Item2, argumentDetails.Item3);

                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured in the application: " + e.ToString());
            }
        }

        private static (string, string, string) GetCommandlineArgs(string[] args)
        {
            string filepath = "", fileNames = "*", outputFilePath = "";
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
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
            }


            //ParseDDL(filepath, fileNames, outputFilePath);
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



            //rawDdl = File.ReadAllText("D:\\ddl transformations\\GeicoDDLTransformers\\docs\\Policy Phase 1 v0.13.52 DDL.ddl");

            rawDdl = File.ReadAllText(filePath);

            var sqlStatements = BuildDdlStatementsCollection(rawDdl);
            string[] fileNameArr = fileNames.Split(',');

            // Debug.Assert(sqlStatements.Count == 3);
            foreach (var sqlStatement in sqlStatements)
            {
                if (!string.IsNullOrWhiteSpace(sqlStatement))
                {
                    if (Array.Exists(fileNameArr, element => element == "hub" || element == "*"))
                        if (sqlStatement.Contains("CREATE TABLE HUB", StringComparison.OrdinalIgnoreCase))
                        {
                            //if (sqlStatement.Contains("CREATE TABLE HUB_POLICY"))
                            {
                                GenerateHubFile(sqlStatement, sqlStatements, outputFilePath);
                            }
                        }
                        else
                        {
                            Console.WriteLine("only create table templates supported for now");
                        }
                    if (Array.Exists(fileNameArr, element => element == "lnk" || element == "*"))
                        if (sqlStatement.Contains("CREATE TABLE LNK", StringComparison.OrdinalIgnoreCase))
                        {

                            // if (sqlStatement.Contains("CREATE TABLE LNK_POLICY_INSURES_VEHICLE"))

                            {
                                GenerateLinkFile(sqlStatement, sqlStatements, outputFilePath);
                            }
                        }
                        else
                        {
                            Console.WriteLine("only create table templates supported for now");
                        }
                    if (Array.Exists(fileNameArr, element => element == "sat" || element == "*"))
                        if (sqlStatement.Contains("CREATE TABLE SAT", StringComparison.OrdinalIgnoreCase))
                        {
                            //if (sqlStatement.Contains("CREATE TABLE SAT_PEAK_POLICY"))
                            {
                                GenerateSatFile(sqlStatement, sqlStatements, outputFilePath);
                            }
                        }
                        else
                        {
                            Console.WriteLine("only create table templates supported for now");
                        }
                }
            }
        }

        private static List<string> BuildDdlStatementsCollection(string str)
        {
            //str = str.Replace(Environment.NewLine, " ");
            var sqlStatements = str.Split(";" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            // var sqlStatements = str.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            //sqlStatements.ForEach(e => e.Trim());
            return sqlStatements;
        }


        private static void GenerateSatFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Console.WriteLine("generating file for table " + tableName);

                var satTableMetadata = new SatTableMetadata()
                {
                    TableName = tableName,
                    SourceModel = "stg_???",
                    Columns = GetDdlStatementColumns(sqlStatement),
                    SrcPk = GetPrimaryKey(sqlStatements, tableName),
                    SrcHashDiff = "HASHDIFF",
                    SrcEff = "EFFECTIVEDATE",
                    SrcLdts = "LOAD_TIMESTAMP",
                    SrcSource = "RECORD_SOURCE",
                    SrcFk = GetForeignKeys(sqlStatements, tableName)
                };

                satTableMetadata.SrcPayload = new List<string>();

                foreach (var column in satTableMetadata.Columns)
                {
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
                }

                outputFilePath += "SAT";
                if (!Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }
                var satFileTemplate = new SatFileTemplate(satTableMetadata);
                var content = satFileTemplate.TransformText();
                File.WriteAllText(outputFilePath + "\\" + satTableMetadata.TableName + $".sql", content);
                Console.WriteLine(outputFilePath + "\\" + satTableMetadata.TableName + $".sql" + " File Generated");
            }

            catch (Exception e)
            {
                Console.WriteLine("Error Generarting SAT file for " + tableName + " Exception details: " + e.ToString());

            }
        }

        private static void GenerateHubFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = GetCreateDdlStatementTableName(sqlStatement);
            try

            {
                Console.WriteLine("generating file for table " + tableName);
                var hubTableMetadata = new HubTableMetadata
                {
                    TableName = tableName,
                    Columns = GetDdlStatementColumns(sqlStatement),
                    srcPk = GetPrimaryKey(sqlStatements, tableName),
                    srcLdts = "LOAD_TIMESTAMP",
                    srcSource = "RECORD_SOURCE",
                    SourceModel = "stg_???"
                };

                hubTableMetadata.srcNk = new List<string>();

                foreach (var column in hubTableMetadata.Columns)
                {
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
                }

                outputFilePath += "HUB";
                if (!Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }
                var hubFileTemplate = new HubFileTemplate(hubTableMetadata);
                var content = hubFileTemplate.TransformText();
                File.WriteAllText(outputFilePath + "\\" + hubTableMetadata.TableName + $".sql", content);
                Console.WriteLine(outputFilePath + "\\" + hubTableMetadata.TableName + $".sql" + " File Generated");
            }
            catch (Exception e)
            {

                Console.WriteLine("Error Generarting HUB file for " + tableName + " Exception details: " + e.ToString());
            }
        }

        private static void GenerateLinkFile(string sqlStatement, List<string> sqlStatements, string outputFilePath)
        {
            var tableName = GetCreateDdlStatementTableName(sqlStatement);
            try
            {
                Console.WriteLine("generating file for table " + tableName);
                var linkTableMetadata = new LinkTableMetadata()
                {
                    TableName = tableName,
                    Columns = GetDdlStatementColumns(sqlStatement),
                    SrcPk = GetPrimaryKey(sqlStatements, tableName),
                    SrcLdts = "LOAD_TIMESTAMP",
                    SrcSource = "RECORD_SOURCE",
                    SourceModel = "stg_???",
                    SrcFk = GetForeignKeys(sqlStatements, tableName)
                };

                outputFilePath += "LNK";
                if (!Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }
                var linkFileTemplate = new LinkFileTemplate(linkTableMetadata);
                var content = linkFileTemplate.TransformText();
                File.WriteAllText(outputFilePath + "\\" + linkTableMetadata.TableName + $".sql", content);
                Console.WriteLine(outputFilePath + "\\" + linkTableMetadata.TableName + $".sql" + " File Generated");
            }

            catch (Exception e)

            {
                Console.WriteLine("Error Generarting LNK file for " + tableName + " Exception details: " + e.ToString());
            }
        }



        private static List<string> GetPrimaryKey(List<string> sqlStatements, string tableName)
        {

            //if (tableName == "SAT_PEAK_VEHICLE" || tableName == "SAT_PEAK_POLICY")
            //    System.Diagnostics.Debugger.Break();



            var primaryKeySearchString = $"ALTER TABLE {tableName}" + Environment.NewLine + "ADD PRIMARY KEY";



            //var sasd = sqlStatements.Single(e => e.Contains(xx));



            //string primaryKeyStatement = sqlStatements.SingleOrDefault(e => e.Contains($"ALTER TABLE {tableName}") && e.Contains("ADD PRIMARY KEY"));
            //string primaryKey = "";



            string primaryKeyStatement = sqlStatements.Single(e => e.Contains(primaryKeySearchString));
            List<string> primaryKeys = new List<string>();
            string primaryKey = "";



            if (!string.IsNullOrWhiteSpace(primaryKeyStatement))
            {
                var pFrom = primaryKeyStatement.IndexOf("(", StringComparison.Ordinal) + 1;
                var pTo = primaryKeyStatement.IndexOf(")", StringComparison.Ordinal);
                primaryKey = primaryKeyStatement.Substring(pFrom, pTo - pFrom);



                if (primaryKey.Contains(","))
                {
                    var primaryKeyArray = primaryKey.Split(",").ToList();

                    foreach (var key in primaryKeyArray)

                    {
                        primaryKeys.Add(key);
                    }

                }

                else
                {
                    primaryKeys.Add(primaryKey);
                }
            }
            return primaryKeys;
        }


        private static List<string> GetForeignKeys(List<string> sqlStatements, string tableName)
        {
            var foreignKeyStatements = sqlStatements.Where(e => e.Contains($"ALTER TABLE {tableName}" + Environment.NewLine) && e.Contains("FOREIGN KEY")).ToList();

            List<string> foreignKeys = new List<string>();

            foreach (var foreignKeyStatement in foreignKeyStatements)
            {
                if (!string.IsNullOrWhiteSpace(foreignKeyStatement))
                {
                    var pFrom = foreignKeyStatement.IndexOf("(", StringComparison.Ordinal) + 1;
                    var pTo = foreignKeyStatement.IndexOf(")", StringComparison.Ordinal);
                    var foreignKey = foreignKeyStatement.Substring(pFrom, pTo - pFrom);
                    foreignKeys.Add(foreignKey);
                }
            }

            return foreignKeys;
        }

        private static List<ColumnDetail> GetDdlStatementColumns(string str)
        {
            //POLICY_NUMBER        VARCHAR(50,0) NULL
            var pFrom = str.IndexOf("(", StringComparison.Ordinal) + 1;
            var pTo = str.LastIndexOf(")", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            var ddlColumns = result.Split("," + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            //TODO: remove the List conversion iterate the array directly.
            var columns = ddlColumns.Select(ddlColumn => ddlColumn.Trim()).ToList();

            //do we need the data types ?, we need to analyse on how to split col name and data types
            // option 1. store the data types in a list and foreach column line in ddl, find and replace the datatype string with "empty" to get the column name. 
            //option 2. find the first occurence of the space and the col name = columnline - first occurence of space and then for datatype

            var columnDetails = new List<ColumnDetail>();
            //Option 2. implementation.
            foreach (var column in columns)
            {
                pTo = column.IndexOf(" ", StringComparison.Ordinal);

                var columnName = column.Substring(0, pTo).Trim();
                var columnDataType = column.Substring(pTo).Trim();

                if (!columnDataType.Contains(")"))
                {
                    var xx = column.Substring(columnName.Length);
                }

                var columnDetail = new ColumnDetail { DataType = columnDataType, Name = columnName };

                columnDetails.Add(columnDetail);
            }

            return columnDetails;
        }

        private static string GetCreateDdlStatementTableName(string str)
        {
            var pFrom = str.IndexOf("CREATE TABLE", StringComparison.Ordinal) + "CREATE TABLE".Length;
            var pTo = str.IndexOf("(", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            return result.Trim();
        }
    }
}