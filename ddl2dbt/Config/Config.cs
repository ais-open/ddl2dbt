using System.Collections.Generic;

namespace ddl2dbt.Config
{
    internal class Config
    {
        public IEnumerable<CSVFileSettings> CSVFileSettings { get; set; }
    }
}