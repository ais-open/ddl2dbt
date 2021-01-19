using System;
using System.Collections.Generic;
using System.Text;

namespace DDLParser.Templates
{
    public partial class HubFileTemplate
    {
        public HubTableMetadata CreateTable;

        public HubFileTemplate(HubTableMetadata createTable)
        {
            CreateTable = createTable;
        }
    }
}
