using System.Collections.Generic;

namespace DDLParser.TemplateModels
{
    public class HubTableMetadata
    {
        public string TableName;
        public List<string> srcPk;
        public List<string> srcNk;
        public string srcLdts;
        public string srcSource;
        public string SourceModel;
        public List<ColumnDetail> Columns;
        public string[] Tags;
    }
}