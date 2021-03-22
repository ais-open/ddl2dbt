using System.Collections.Generic;

namespace ddl2dbt.TemplateModels
{
    public class StgMetadata
    {
        public string TableName { get; set; }

        public string SourceModelValue { get; set; }

        public string SourceModelLabel { get; set; }

        public List<LabelListPair> DerivedColumns { get; set; }

        public List<LabelListPair> HashedColumns { get; set; }

        public bool HashDiff { get; set; }
        public bool IsFIleTypeBR { get; set; }

        public List<string> Columns { get; set; }

        public string[] Tags { get; set; }
    }
}