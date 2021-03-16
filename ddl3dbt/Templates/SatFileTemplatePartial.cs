using ddl3dbt.TemplateModels;

namespace ddl3dbt.Templates
{
    public partial class SatFileTemplate
    {
        public SatTableMetadata SatTableMetadata;

        public SatFileTemplate(SatTableMetadata satTableMetadata)
        {
            SatTableMetadata = satTableMetadata;
        }
    }
}
