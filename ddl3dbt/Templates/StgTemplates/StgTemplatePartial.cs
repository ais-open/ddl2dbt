using ddl3dbt.TemplateModels;

namespace ddl3dbt.Templates.StgTemplates
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
