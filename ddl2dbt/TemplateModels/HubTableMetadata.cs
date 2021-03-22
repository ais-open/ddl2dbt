using System.Collections.Generic;

namespace ddl2dbt.TemplateModels
{
    public class HubTableMetadata
    {
        public string TableName { get; set; }
        public string SrcPk { get; set; }
        public List<string> SrcNk { get; set; }
        public string SrcLdts { get; set; }
        public string SrcSource { get; set; }
        public List<string> SourceModel { get; set; }
        public List<ColumnDetail> Columns { get; set; }
        public string[] Tags { get; set; }
        public List<string> PrimaryKeys { get; set; }
    }
}