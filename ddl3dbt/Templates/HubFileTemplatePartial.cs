using ddl3dbt.TemplateModels;

namespace ddl3dbt.Templates
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
