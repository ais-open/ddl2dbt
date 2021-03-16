using ddl2dbt.TemplateModels;

namespace ddl2dbt.Templates
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