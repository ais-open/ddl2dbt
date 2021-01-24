using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatPeakVehicleTemplate
    {
        public StgMetadata StgMetadata;
        public SatPeakVehicleTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
