using System.Collections.Generic;

namespace ddl3dbt.TemplateModels
{
    public class StgMetadata
    {
        public string TableName { get; set; }

        public string SourceModelValue { get; set; }

        public string SourceModelLabel { get; set; }

        public List<LabelListPair> DerivedColumns { get; set; }

        //public List<LabelValuePair> HashedColumns { get; set; }

        public List<LabelListPair> HashedColumns { get; set; }

        public bool HashDiff { get; set; }
        public bool IsFIleTypeBR { get; set; }

        public List<string> Columns { get; set; }

        public string[] Tags { get; set; }
        public bool hasDerivedColumns { get; set; }
        public string PrimaryKey { get; set; }
        public List<string> Compositekeys { get; set; }
        public bool CompositeKeysPresent { get; set; }
    }
}