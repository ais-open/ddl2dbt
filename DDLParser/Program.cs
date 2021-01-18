using System;
using System.Collections.Generic;
using System.Linq;

namespace DDLParser
{
    internal class Program
    {
        private static void Main()
        {

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
POLICY_NUMBER        VARCHAR(50) NULL
);
 
ALTER TABLE HUB_POLICY
ADD PRIMARY KEY (POLICY_HK);";

            rawDdl = rawDdl.Replace(Environment.NewLine, "");
            var sqlStatements = SplitSql(rawDdl);

            // Debug.Assert(sqlStatements.Count == 3);
            foreach (var sqlStatement in sqlStatements)
                if (!string.IsNullOrWhiteSpace(sqlStatement))
                    if (sqlStatement.Contains("CREATE TABLE HUB", StringComparison.OrdinalIgnoreCase))
                    {
                        if (sqlStatement.Contains("CREATE TABLE HUB_POLICY"))
                        {
                            GenerateOutputHubPolicyFile(sqlStatement);
                        }
                        else if (sqlStatement.Contains("CREATE TABLE HUB_VEHICLE"))
                        {

                        }
                        else if (sqlStatement.Contains("CREATE TABLE HUB_COVERAGE"))
                        {

                        }
                    }
                    else
                    {
                        Console.WriteLine("only create table templates supported for now");
                    }
        }

        private static List<string> SplitSql(string str)
        {
            return str.Split(";").ToList();
        }

        private static void GenerateOutputHubPolicyFile(string str)
        {
            var createTable = new CreateTable
            {
                TableName = GetCreateDdlStatementTableName(str),
                Columns = GetDdlStatementColumns(str),
                //TODO: Get mapping details, how to determine the mapping

                LoadTimestamp = "LOAD_TIMESTAMP",
                PolicyHk = "POLICY_HK",
                PolicyNumber = "POLICY_NUMBER",
                RecordSource = "RECORD_SOURCE"
            };
            var runtimeTextTemplate1 = new RuntimeTextTemplate1(createTable);

            var content = runtimeTextTemplate1.TransformText();
            System.IO.File.WriteAllText(createTable.TableName+$".sql", content);
            Console.WriteLine("File Generated");
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
    }
}