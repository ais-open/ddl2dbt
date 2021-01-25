using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatPeakRiskCoverageTemplate
    {
        public StgMetadata StgMetadata;

        public SatPeakRiskCoverageTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
