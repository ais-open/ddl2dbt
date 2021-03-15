﻿using System;
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
            Map(m => m.Tags).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Tags", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.TableDefinition).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Table Description", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.ColumnName).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Column Name", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.HashdiffColumns).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Hashdiff Columns", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.ColumnDefinition).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Column Description", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.SourceModel).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Source-Model", StringComparison.OrdinalIgnoreCase)).FieldValue);
            Map(m => m.StageColumns).Name(_config.CSVFileSettings.Single(e => string.Equals(e.FieldName, "Stage-Columns", StringComparison.OrdinalIgnoreCase)).FieldValue);
        }
    }
}
