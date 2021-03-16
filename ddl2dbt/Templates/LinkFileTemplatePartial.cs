using ddl2dbt.TemplateModels;

namespace ddl2dbt.Templates
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
