using System;
using System.Collections.Generic;
using System.Text;
using DDLParser.TemplateModels;

namespace DDLParser.Templates.StgTemplates
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
