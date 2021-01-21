using System.Collections.Generic;

namespace DDLParser.TemplateModels
{
    public class LinkTableMetadata
    {
        public string TableName;
        public string SrcLdts;
        public string SrcSource;
        public List<string> SrcPk;
        public string SourceModel;
        public List<string> SrcFk;
        public List<ColumnDetail> Columns;
        public string[] Tags;
    }
}