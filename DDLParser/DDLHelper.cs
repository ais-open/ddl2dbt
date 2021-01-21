using System;
using System.Collections.Generic;
using System.Linq;

namespace DDLParser
{
    internal static class DDLParserHelper
    {
        public static List<string> GetPrimaryKey(List<string> sqlStatements, string tableName)
        {
            //if (tableName == "SAT_PEAK_VEHICLE" || tableName == "SAT_PEAK_POLICY")
            //    System.Diagnostics.Debugger.Break();


            var primaryKeySearchString = $"ALTER TABLE {tableName}" + Environment.NewLine + "ADD PRIMARY KEY";


            //var sasd = sqlStatements.Single(e => e.Contains(xx));


            //string primaryKeyStatement = sqlStatements.SingleOrDefault(e => e.Contains($"ALTER TABLE {tableName}") && e.Contains("ADD PRIMARY KEY"));
            //string primaryKey = "";


            var primaryKeyStatement = sqlStatements.Single(e => e.Contains(primaryKeySearchString));
            var primaryKeys = new List<string>();


            if (!string.IsNullOrWhiteSpace(primaryKeyStatement))
            {
                var pFrom = primaryKeyStatement.IndexOf("(", StringComparison.Ordinal) + 1;
                var pTo = primaryKeyStatement.IndexOf(")", StringComparison.Ordinal);
                var primaryKey = primaryKeyStatement.Substring(pFrom, pTo - pFrom);


                if (primaryKey.Contains(","))
                {
                    var primaryKeyArray = primaryKey.Split(",").ToList();

                    foreach (var key in primaryKeyArray) primaryKeys.Add(key);
                }

                else
                {
                    primaryKeys.Add(primaryKey);
                }
            }

            return primaryKeys;
        }

        public static List<string> BuildDdlStatementsCollection(string str)
        {
            //str = str.Replace(Environment.NewLine, " ");
            var sqlStatements = str.Split(";" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            // var sqlStatements = str.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
            //sqlStatements.ForEach(e => e.Trim());
            return sqlStatements;
        }

        public static List<string> GetForeignKeys(List<string> sqlStatements, string tableName)
        {
            var foreignKeyStatements = sqlStatements.Where(e =>
                e.Contains($"ALTER TABLE {tableName}" + Environment.NewLine) && e.Contains("FOREIGN KEY")).ToList();

            var foreignKeys = new List<string>();

            foreach (var foreignKeyStatement in foreignKeyStatements)
                if (!string.IsNullOrWhiteSpace(foreignKeyStatement))
                {
                    var pFrom = foreignKeyStatement.IndexOf("(", StringComparison.Ordinal) + 1;
                    var pTo = foreignKeyStatement.IndexOf(")", StringComparison.Ordinal);
                    var foreignKey = foreignKeyStatement.Substring(pFrom, pTo - pFrom);
                    foreignKeys.Add(foreignKey);
                }

            return foreignKeys;
        }

        public static List<ColumnDetail> GetDdlStatementColumns(string str)
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

                var columnDetail = new ColumnDetail {DataType = columnDataType, Name = columnName};

                columnDetails.Add(columnDetail);
            }

            return columnDetails;
        }

        public static string GetCreateDdlStatementTableName(string str)
        {
            var pFrom = str.IndexOf("CREATE TABLE", StringComparison.Ordinal) + "CREATE TABLE".Length;
            var pTo = str.IndexOf("(", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            return result.Trim();
        }
    }
}