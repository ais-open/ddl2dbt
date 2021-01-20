using System.Collections.Generic;

namespace DDLParser
{
    //TOOD: change the names to match the dbValut names from the template.
    public class HubTableMetadata
    {
        public string TableName;
        public List<string> srcPk;
        public List<string> srcNk;
        public string srcLdts;
        public string srcSource;
        public string SourceModel;
        public List<ColumnDetail> Columns;
        public List<string> ForeignKeys;
    }

    public class ColumnDetail
    {
        public string Name;
        public string DataType;

    }

    public class LinkTableMetadata
    {
        public string TableName;
        public string SrcLdts;
        public string SrcSource;
        public List<string> SrcPk;
        public string SourceModel;
        public List<string> SrcFk;
        public List<ColumnDetail> Columns;
    }


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