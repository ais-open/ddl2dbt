using ddl2dbt.TemplateModels;

namespace ddl2dbt.Templates
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
