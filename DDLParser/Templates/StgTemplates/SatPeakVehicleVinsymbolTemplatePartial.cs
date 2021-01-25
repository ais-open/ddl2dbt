using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatPeakVehicleVinsymbolTemplate
    {
        public StgMetadata StgMetadata;

        public SatPeakVehicleVinsymbolTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
