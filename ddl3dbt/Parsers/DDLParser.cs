using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ddl3dbt.Logging;
using ddl3dbt.TemplateModels;

namespace ddl3dbt.Parsers
{
    internal static class DDLParser
    {
        public static List<string> GetPrimaryKey(List<string> sqlStatements, string tableName)
        {
            var primaryKeySearchString = $"ALTER TABLE {tableName}" + Environment.NewLine + "ADD PRIMARY KEY";
            var primaryKeyStatement = sqlStatements.Single(e => e.Contains(primaryKeySearchString,StringComparison.OrdinalIgnoreCase));
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

        public static List<string> BuildDdlStatementsCollection(string ddlFilePath)
        {
            if (ddlFilePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || 
                ddlFilePath.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) ||
                ddlFilePath.EndsWith(".ddl", StringComparison.OrdinalIgnoreCase)
                )
            { }
            else
                throw new Exception(Constants.InvalidDDLFileExensionError);

            if (!File.Exists(ddlFilePath)) 
            {
                throw new Exception(Constants.InvalidDDLFileLocation); 
            }

            var rawddl = File.ReadAllText(ddlFilePath);

            var sqlStatements = rawddl.Split(";" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (sqlStatements == null || !sqlStatements.Any())
                throw new Exception(Constants.InvalidDDLFileError);
            if(!rawddl.Contains("create table",StringComparison.OrdinalIgnoreCase))
                throw new Exception(Constants.InvalidDDlFileCouldNotFindCreateTable);

            return sqlStatements;
        }

        public static List<string> GetForeignKeys(List<string> sqlStatements, string tableName)
        {
            var foreignKeyStatements = sqlStatements.Where(e =>
                e.Contains($"ALTER TABLE {tableName}" + Environment.NewLine,StringComparison.OrdinalIgnoreCase) && e.Contains("FOREIGN KEY")).ToList();

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
            var columnDetails = new List<ColumnDetail>();
            try
            {
                var pFrom = str.IndexOf("(", StringComparison.Ordinal) + 1;
                var pTo = str.LastIndexOf(")", StringComparison.Ordinal);
                string result = null;
                if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

                var ddlColumns = result.Split("," + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var columns = ddlColumns.Select(ddlColumn => ddlColumn.Trim()).ToList();

                foreach (var column in columns)
                {

                    pTo = column.IndexOf(" ", StringComparison.Ordinal);

                    var columnName = column.Substring(0, pTo).Trim();
                    var columnDataType = column.Substring(pTo).Trim();

                    var columnDetail = new ColumnDetail { DataType = columnDataType, Name = columnName };

                    columnDetails.Add(columnDetail);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error extracting column details for ddl statement: {str}, please check log for more details.");
            }

            return columnDetails;
        }

        public static string GetCreateDdlStatementTableName(string str)
        {
            var pFrom = str.IndexOf("CREATE TABLE", StringComparison.Ordinal) + "CREATE TABLE".Length;
            var pTo = str.IndexOf("(", StringComparison.Ordinal);
            string result = null;
            if (pFrom >= 0 && str.Length > pFrom) result = str.Substring(pFrom, pTo - pFrom);

            return result?.Trim();
        }

    }
}
