using System.Collections.Generic;

namespace DDL2Dbt.Config
{
    public class Config
    {
        public IEnumerable<HubFileGenerationSetting> HubFileGenerationSettings { get; set; }
        public IEnumerable<SatFileGenerationSetting> SatFileGenerationSettings { get; set; }
        public IEnumerable<LnkFileGenerationSetting> LnkFileGenerationSettings { get; set; }
    }
}