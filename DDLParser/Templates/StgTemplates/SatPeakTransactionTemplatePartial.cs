using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class SatPeakTransactionTemplate
    {
        public StgMetadata StgMetadata;
        public SatPeakTransactionTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
