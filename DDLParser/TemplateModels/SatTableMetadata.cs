using System.Collections.Generic;

namespace DDLParser.TemplateModels
{
    //TOOD: change the names to match the dbValut names from the template.


    public class SatTableMetadata
    {
        public string TableName;
        public string SourceModel;
        public List<string> SrcPk;
        public string SrcHashDiff;
        public string SrcEff;
        public string SrcLdts;
        public string SrcSource;
        public List<string> SrcPayload;
        public List<string> SrcFk;
        public List<ColumnDetail> Columns;
    }

}