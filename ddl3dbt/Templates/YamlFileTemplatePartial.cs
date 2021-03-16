using ddl3dbt.TemplateModels;

namespace ddl3dbt.Templates
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