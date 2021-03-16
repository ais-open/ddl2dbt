using System.Collections.Generic;

namespace ddl3dbt.TemplateModels
{
    //TOOD: change the names to match the dbValut names from the template.


    public class SatTableMetadata
    {
        public string TableName { get; set; }

        public string SourceModel { get; set; }

        public string SrcPk { get; set; }

        public string SrcHashDiff { get; set; }

        public string SrcEff { get; set; }

        public string SrcLdts { get; set; }

        public string SrcSource { get; set; }

        public List<string> SrcPayload { get; set; }

        public List<string> SrcFk { get; set; }

        public List<ColumnDetail> Columns { get; set; }

        public string[] Tags { get; set; }

        public List<string> PrimaryKeys { get; set; }

    }

}