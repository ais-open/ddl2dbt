using CsvHelper.Configuration.Attributes;

namespace DDL2Dbt.Parsers
{
    public class CsvDataSource
    {
        //[Name("Table Name")]
        public string TableName { get; set; }

        //[Name("Entity.Physical.OWNER_TEAM_NAME")]
        public string PhysicalQwnerTeamName { get; set; }

        [Optional]
        //[Name("Table Definition")]
        public string TableDefinition { get; set; }

        //[Name("Column Name")]
        public string ColumnName { get; set; }

        //[Name("Attribute.Physical.INCLUDE_IN_HASHDIFF")]
        public string IncludeInHashdiff { get; set; }

        [Optional]
        //[Name("Attribute.Physical.SPI_CLASSIFICATION")]
        public string SPIClassification { get; set; }

        [Optional]
        //[Name("Column Data_Type")]
        public string ColumnDataType { get; set; }

        [Optional]
        //[Name("Column Definition")]
        public string ColumnDefinition { get; set; }

        //[Name("Table4.SourceTable")]
        public string Table4SourceTable { get; set; }

        [Optional]
        //[Name("Table4.TargetTable")]
        public string Table4TargetTable { get; set; }

        [Optional]
        //[Name("Table4.RecordSource")]
        public string Table4RecordSource { get; set; }

        [Optional]
        //[Name("Table5.SourceTable")]
        public string Table5SourceTable { get; set; }

        //[Name("Table5.SourceTransform")]
        public string Table5SourceTransform { get; set; }

        [Optional]
        //[Name("Table5.TargetColumn")]
        public string Table5TargetColumn { get; set; }

    }
}