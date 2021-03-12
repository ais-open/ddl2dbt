using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates
{
    public partial class YamlFileTemplate
    {
        public YamlFileMetadata YamlFileMetadata;


        public YamlFileTemplate(YamlFileMetadata yamlFileMetadata)
        {
            YamlFileMetadata = yamlFileMetadata;
        }
    }
}