using CsvHelper.Configuration.Attributes;

namespace ddl2dbt.Parsers
{
    public class CsvDataSource
    {
        public string TableName { get; set; }

        public string Tags { get; set; }

        public string TableDefinition { get; set; }

        public string ColumnName { get; set; }

        public string HashdiffColumns { get; set; }

        public string ColumnDefinition { get; set; }

        public string SourceModel { get; set; }

        public string StageColumns { get; set; }
        public string NullOption { get; set; }
        public string PrimaryKey { get; set; }
        public string ForeignKey { get; set; }

    }
}