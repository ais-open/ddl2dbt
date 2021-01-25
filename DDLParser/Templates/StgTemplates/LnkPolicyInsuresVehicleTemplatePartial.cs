using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class LnkPolicyInsuresVehicleTemplate
    {
        public StgMetadata StgMetadata;
        public LnkPolicyInsuresVehicleTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
