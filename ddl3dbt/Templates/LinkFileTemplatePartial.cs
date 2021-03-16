using ddl3dbt.TemplateModels;

namespace ddl3dbt.Templates
{
    public partial class LinkFileTemplate
    {
        public LinkTableMetadata LinkTableMetadata;

        public LinkFileTemplate(LinkTableMetadata linkTableMetadata)
        {
            LinkTableMetadata = linkTableMetadata;
        }
    }
}
