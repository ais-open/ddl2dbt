using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using DDL2Dbt.Config;

namespace DDL2Dbt.Parsers
{
    public sealed class CSVDataSourceMap : ClassMap<CsvDataSource>
    {
        private static Config.Config _config;
        public CSVDataSourceMap()
        {
            _config = ConfigurationProvider.GetConfigSettings();
            Map(m => m.TableName).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table Name", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.PhysicalQwnerTeamName).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Tags", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.TableDefinition).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table Description", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.ColumnName).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Column Name", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.IncludeInHashdiff).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Hashdiff Columns", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.SPIClassification).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Spi Classification", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.ColumnDataType).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Column Data-Type", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.ColumnDefinition).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Column Description", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table4SourceTable).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-4 Source Table", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table4TargetTable).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-4 Target Table", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table4RecordSource).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-4 Record Source", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table5SourceTable).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-5 Source Table", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table5SourceTransform).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-5 Source Transform", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.Table5TargetColumn).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table-5 Target Column", StringComparison.OrdinalIgnoreCase)).FieldValue);
        }
    }
}
