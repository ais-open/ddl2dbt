using System;
using System.Collections.Generic;
using System.Text;

namespace DDLParser.Templates
{
    public partial class HubFileTemplate
    {
        public HubTableMetadata HubTableMetadata;

        public HubFileTemplate(HubTableMetadata hubTableMetadata)
        {
            HubTableMetadata = hubTableMetadata;
        }
    }
}
