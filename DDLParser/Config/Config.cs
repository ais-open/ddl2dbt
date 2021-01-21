using System.Collections.Generic;

namespace DDLParser
{
    public class Config
    {
        public IEnumerable<HubFileGenerationSetting> HubFileGenerationSettings { get; set; }
        public IEnumerable<SatFileGenerationSetting> SatFileGenerationSettings { get; set; }
        public IEnumerable<LnkFileGenerationSetting> LnkFileGenerationSettings { get; set; }
    }
}