using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using ddl2dbt.Logging;

namespace ddl2dbt.Parsers
{
    internal class CsvParser
    {
        public static List<CsvDataSource> ParseCsv(string csvFilePath, string fileType)
        {
            List<CsvDataSource> records = null;
            try
            {
                if((fileType.Contains("stg",StringComparison.OrdinalIgnoreCase)|| fileType.Contains("*", StringComparison.OrdinalIgnoreCase)) && string.IsNullOrWhiteSpace(csvFilePath))
                    Logger.LogWarning("csv file needs to be provided to generate stg files");

                if (!string.IsNullOrWhiteSpace(csvFilePath))
                {
                    using var reader = new StreamReader(csvFilePath);
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture, false))
                    {
                        csv.Context.RegisterClassMap<CSVDataSourceMap>();
                        records = csv.GetRecords<CsvDataSource>().ToList();
                    }

                    if (!records.Any())
                        throw new Exception(Constants.InvalidCSVFileError);
                }
            }
            catch (Exception ex)
            {
                Logger.LogVerbose(ex, Constants.InvalidCSVFileError);
                // The default exception's message from the csvparser library contains a lot of information, if presented to the user would be confusing
                // So throwing a new exception with a simple message.
                throw new Exception(Constants.InvalidCSVFileError);
            }
            return records;
        }


        public static string[] GetTags(List<CsvDataSource> csvDataSources, string tableName)
        {
            var tags = new string[] { Constants.NotFoundString };

            if (csvDataSources != null)
            {
                foreach (var csvDataSource in csvDataSources)
                {
                    if (csvDataSource.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrWhiteSpace(csvDataSource.Tags))
                        {
                            tags = csvDataSource.Tags.Split(",");
                        }
                    }
                }
            }

            if (tags.Length == 1 && tags[0].Equals(Constants.NotFoundString, StringComparison.OrdinalIgnoreCase))
                Logger.LogWarning(Constants.CouldNotFindTagInConfig + tableName);
            return tags;

        }

    }
}