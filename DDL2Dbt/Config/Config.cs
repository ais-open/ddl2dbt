using System.Collections.Generic;

namespace DDL2Dbt.Config
{
    internal class Config
    {
        public IEnumerable<CSVFileSettings> CSVFileSettings { get; set; }
    }
}