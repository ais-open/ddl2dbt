using ddl2dbt.TemplateModels;

namespace ddl2dbt.Templates
{
    public partial class HubFileTemplate
    {
        public HubTableMetadata HubTableMetadata;

        public HubFileTemplate(HubTableMetadata hubTableMetadata)
        {
            HubTableMetadata = hubTableMetadata;
        }
    }
}
