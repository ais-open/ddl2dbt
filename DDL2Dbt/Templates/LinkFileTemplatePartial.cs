using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates
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
