using System.Collections.Generic;

namespace ddl2dbt.TemplateModels
{
    //This object is used in populating the derived columns of the stage files.
    public class LabelListPair
    {
        public string Label { get; set; }
        public List<string> Value { get; set; }
    }
}
