using ddl2dbt.TemplateModels;

namespace ddl2dbt.Templates.StgTemplates
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
