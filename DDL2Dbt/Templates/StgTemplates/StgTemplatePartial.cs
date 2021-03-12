using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class StgTemplate
    {
        public StgMetadata StgMetadata;

        public StgTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
