using CsvHelper.Configuration.Attributes;

namespace DDL2Dbt
{
    class DataSource
    {
        [Name("Table Name")]
        public string TableName { get; set; }

        [Name("Column Name")]
        public string ColumnName { get; set; }

        [Name("Column Data_Type")]
        public string ColumnDataType { get; set; }

        [Name("Data Source Column/Data Movement Column Name")]
        public string DataSourceColumnName { get; set; }

        [Name("Data Source Column/Data Movement Column Physical_Data_Type")]
        public string DataSourceColumnDataType { get; set; }

        [Name("Data Source Table/Data Movement Table Name")]
        public string DataSourceTableName { get; set; }

        [Name("Data Source Object Name")]
        public string DataSourceObjectName { get; set; }

        [Name("Data Source Object Data_Source_Import_Type")]
        public string DataSourceObjectImportType { get; set; }

        [Name("Data Source Object Data_Source_Server")]
        public string DataSourceObjectServer { get; set; }

        [Name("Data Source Object Data_Source_System")]
        public string DataSourceObjectSystem { get; set; }

        [Name("Data Source Object Data_Source_Type")]
        public string DataSourceObjectType { get; set; }
    }
}
