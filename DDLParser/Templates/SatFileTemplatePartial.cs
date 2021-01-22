using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates
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
