using System;
using System.Collections.Generic;
using System.Text;

namespace DDL2Dbt.TemplateModels
{
    public class YamlFileMetadata
    {
        public string TableName { get; set; }
        public string TableDefinition { get; set; }
        public List<LabelValuePair> ColumnDefinition { get; set; }
    }
}
