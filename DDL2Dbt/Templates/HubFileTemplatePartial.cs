using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates
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
