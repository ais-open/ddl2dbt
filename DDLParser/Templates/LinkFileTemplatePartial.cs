using DDLParser.TemplateModels;

namespace DDLParser.Templates
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
