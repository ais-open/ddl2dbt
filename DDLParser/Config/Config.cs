using System.Collections.Generic;

namespace DDLParser
{
    public class Config
    {
        public IEnumerable<HubFileGenerationSetting> HubFileGenerationSettings { get; set; }
    }
}