using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ddl2dbt.Logging;
using ddl2dbt.TemplateModels;

namespace ddl2dbt.Parsers
{
    internal static class DDLParser
    {
        public static List<string> GetPrimaryKey(List<string> sqlStatements, string tableName, List<CsvDataSource> records)
        {
            var primaryKeys = new List<string>();
            var tableRecords = new List<CsvDataSource>();
            if (records != null && records.Any())
            {
                tableRecords = records.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
             foreach (var record in tableRecords)
            {
                if (record.PrimaryKey.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                {
                    primaryKeys.Add(record.ColumnName);
                }
            }

            if (primaryKeys == null || !primaryKeys.Any())
            {
                Logger.LogWarning("Could not find Primary Key in the csv file for Table: " + tableName);
                primaryKeys = new List<string> { Constants.NotFoundString };
            }

            return primaryKeys.Distinct().ToList();
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

        public static List<string> GetForeignKeys(List<string> sqlStatements, string tableName, List<CsvDataSource> records)
        {
            var foreignKeys = new List<string>();
            var tableRecords = new List<CsvDataSource>();
            if (records != null && records.Any())
            {
                tableRecords = records.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            foreach (var record in tableRecords)
            {
                if (record.ForeignKey.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                {
                    foreignKeys.Add(record.ColumnName);
                }
            }

            if (foreignKeys == null || !foreignKeys.Any())
            {
                Logger.LogWarning("Could not find Foreign Key in the csv file for Table: " + tableName);
                foreignKeys = new List<string> { Constants.NotFoundString };
            }

            return foreignKeys.Distinct().ToList();
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
