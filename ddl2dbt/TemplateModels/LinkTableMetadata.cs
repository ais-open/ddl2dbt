﻿using System.Collections.Generic;

namespace ddl2dbt.TemplateModels
{
    public class LinkTableMetadata
    {
        public string TableName { get; set; }
        public string SrcLdts { get; set; }
        public string SrcSource { get; set; }
        public string SrcPk { get; set; }
        public List<string> SourceModel { get; set; }
        public List<string> SrcFk { get; set; }

        public List<ColumnDetail> Columns { get; set; }

        public string[] Tags { get; set; }

        public List<string> PrimaryKeys { get; set; }
        public bool MaskedColumnsPresent { get; set; }
        public List<LabelValuePair> MaskedColumns { get; set; }

    }
}