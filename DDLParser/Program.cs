using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DDLParser.Templates;

namespace DDLParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //if (!File.Exists(args[0]))
            //{
            //    Console.WriteLine("file not found");
            //}

            //TODO: implement capturing the input and output folder names for reading the files.

            ParseDDL();
            Console.Read();
        }


        private static void ParseDDL()
        {


            var rawDdl = @"CREATE TABLE HUB_POLICY 
(
POLICY_HK            BINARY() NOT NULL,
LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
RECORD_SOURCE        VARCHAR(100) NULL,
POLICY_NUMBER        VARCHAR(50) NULL,
SAMPLE_FIELD        VARCHAR(50) NULL
);
 
ALTER TABLE HUB_POLICY
ADD PRIMARY KEY (POLICY_HK);";



             rawDdl = File.ReadAllText("D:\\ddl transformations\\GeicoDDLTransformers\\docs\\Policy Phase 1 v0.13.52 DDL.ddl");

            var sqlStatements = BuildDdlStatementsCollection(rawDdl);

            // Debug.Assert(sqlStatements.Count == 3);
            foreach (var sqlStatement in sqlStatements)
                if (!string.IsNullOrWhiteSpace(sqlStatement))
                    if (sqlStatement.Contains("CREATE TABLE HUB", StringComparison.OrdinalIgnoreCase))
                    {
                        if (sqlStatement.Contains("CREATE TABLE HUB"))
                        {
                             GenerateOutputHubPolicyFile(sqlStatement, sqlStatements);
                        }
                        //else if (sqlStatement.Contains("CREATE TABLE HUB_VEHICLE"))
                        //{

                        //}
                        //else if (sqlStatement.Contains("CREATE TABLE HUB_COVERAGE"))
                        //{

                        //}
                    }
                    else if (sqlStatement.Contains("CREATE TABLE LNK", StringComparison.OrdinalIgnoreCase))
                    {

                        if (sqlStatement.Contains("CREATE TABLE LNK_POLICY_INSURES_VEHICLE"))
                        {
                            //GenerateLinkFile(sqlStatement, sqlStatements);
                        }

                    }
                    else
                    {
                        Console.WriteLine("only create table templates supported for now");
                    }
        }

        private static List<string> BuildDdlStatementsCollection(string str)
        {
            str = str.Replace(Environment.NewLine, " ");
            var sqlStatements = str.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            sqlStatements.ForEach(e => e.Trim());
            return sqlStatements;
        }

        private static void GenerateOutputHubPolicyFile(string sqlStatement, List<string> sqlStatements)
        {
            var tableName = GetCreateDdlStatementTableName(sqlStatement);
            var foreignKeys = GetForeignKeys(sqlStatements, tableName);
            var createTable = new HubTableMetadata
            {
                TableName = tableName,
                Columns = GetDdlStatementColumns(sqlStatement),
                srcPk = GetPrimaryKey(sqlStatements, tableName),
                srcLdts = "LOAD_TIMESTAMP",
                srcSource = "RECORD_SOURCE",
                srcNk = GetNaturalKeys(sqlStatement),
                SourceModel = "stg_???"
            };

            var hubFileTemplate = new HubFileTemplate(createTable);
            var content = hubFileTemplate.TransformText();
            File.WriteAllText(createTable.TableName + $".sql", content);
            Console.WriteLine("File Generated");
        }

        private static void GenerateLinkFile(string sqlStatement, List<string> sqlStatements)
        {
            var tableName = GetCreateDdlStatementTableName(sqlStatement);
         
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

            var linkTemplate = new LinkTemplate(linkTableMetadata);
            var content = linkTemplate.TransformText();
            File.WriteAllText(linkTableMetadata.TableName + $".sql", content);
            Console.WriteLine("File Generated");
        }

        private static string GetPrimaryKey(List<string> sqlStatements, string tableName)
        {
            string primaryKeyStatement = sqlStatements.SingleOrDefault(e => e.Contains($"ALTER TABLE {tableName} ADD PRIMARY KEY"));
            string primaryKey = "";

            if (!string.IsNullOrWhiteSpace(primaryKeyStatement))
            {
                var pFrom = primaryKeyStatement.IndexOf("(", StringComparison.Ordinal) + 1;
                var pTo = primaryKeyStatement.IndexOf(")", StringComparison.Ordinal);
                primaryKey = primaryKeyStatement.Substring(pFrom, pTo - pFrom);
            }

            return primaryKey;
        }


        private static List<string> GetNaturalKeys(string str) 
        {
            var pFrom = str.IndexOf("(", StringComparison.Ordinal) + 1;
            var pTo = str.LastIndexOf(")", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            var ddlColumns = result.Split(",");
            //TODO: remove the List conversion iterate the array directly.
            var columns = ddlColumns.Select(ddlColumn => ddlColumn.Trim()).ToList();

            //do we need the data types ?, we need to analyse on how to split col name and data types
            // option 1. store the data types in a list and foreach column line in ddl, find and replace the datatype string with "empty" to get the column name. 
            //option 2. find the first occurence of the space and the col name = columnline - first occurence of space and then for datatype

            var naturalKeys = new List<string>();
            //Option 2. implementation.
            foreach (var column in columns)
            {
                pTo = column.IndexOf(" ", StringComparison.Ordinal);

                var columnName = column.Substring(0, pTo);
                if (columnName != "RECORD_SOURCE" && columnName != "LOAD_TIMESTAMP" && !columnName.EndsWith("HK")) 
                {
                    naturalKeys.Add(columnName);
                }
            }

            return naturalKeys;
        }


        private static List<string> GetForeignKeys(List<string> sqlStatements, string tableName)
        {
            var foreignKeyStatements = sqlStatements.Where(e => e.Contains($"ALTER TABLE {tableName}") && e.Contains("FOREIGN KEY"));
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
            var pFrom = str.IndexOf("(", StringComparison.Ordinal) + 1;
            var pTo = str.LastIndexOf(")", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            var ddlColumns = result.Split(",");
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

                var columnName = column.Substring(0, pTo);
                var columnDataType = column.Substring(pTo).Trim();

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

        private static string GetPrimaryKey(string str, string tableName)
        {
            str = "ALTER TABLE HUB_POLICY ADD PRIMARY KEY (POLICY_HK)";

            //var alterTableStr= $"Hello, {name}! Today is {date.DayOfWeek}, it's {date:HH:mm} now."
            //G

            return "";

        }
    }
}