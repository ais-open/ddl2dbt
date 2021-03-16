using CsvHelper.Configuration.Attributes;

namespace ddl3dbt.Parsers
{
    public class CsvDataSource
    {
        public string TableName { get; set; }

        public string Tags { get; set; }

        [Optional]
        public string TableDefinition { get; set; }

        public string ColumnName { get; set; }

        public string HashdiffColumns { get; set; }

        [Optional]
        public string ColumnDefinition { get; set; }

        public string SourceModel { get; set; }

        public string StageColumns { get; set; }

    }
}