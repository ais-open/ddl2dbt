using System;
using System.Collections.Generic;
using System.Text;
using DDL2Dbt.TemplateModels;

namespace DDL2Dbt.Templates.StgTemplates
{
    public partial class LnkPolicyHasTrancactionTemplate
    {
        public StgMetadata StgMetadata;
        public LnkPolicyHasTrancactionTemplate(StgMetadata stgMetadata)
        {
            StgMetadata = stgMetadata;
        }
    }
}
