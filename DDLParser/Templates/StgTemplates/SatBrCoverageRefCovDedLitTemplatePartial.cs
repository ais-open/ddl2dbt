using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatBrCoverageRefCovDedLitTemplate
    {
        public StgMetadata StgMetadata;

        public SatBrCoverageRefCovDedLitTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
