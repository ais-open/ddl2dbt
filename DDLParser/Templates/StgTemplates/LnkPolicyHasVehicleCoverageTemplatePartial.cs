using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class LnkPolicyHasVehicleCoverageTemplate
    {
        public StgMetadata StgMetadata;

        public LnkPolicyHasVehicleCoverageTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
