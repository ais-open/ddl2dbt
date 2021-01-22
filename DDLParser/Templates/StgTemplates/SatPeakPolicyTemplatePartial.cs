using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatPeakPolicyTemplate
    {
        public StgMetadata StgMetadata;

        public SatPeakPolicyTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
