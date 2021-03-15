using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DDL2Dbt.Logging;
using DDL2Dbt.Parsers;
using DDL2Dbt.TemplateModels;
using DDL2Dbt.Templates.StgTemplates;

namespace DDL2Dbt.ModelFileGenerators
{
    internal class StgFileGenerator
    {
        public static void GenerateFile(string tableName, List<CsvDataSource> csvDataSource,
            string outputFilePath, List<string> primaryKeys)
        {
            var stgMetadata = new StgMetadata();
            try
            {

                //get the records related to the table
                List<CsvDataSource> tableRecords = csvDataSource.Where(e => e.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (tableRecords == null || !tableRecords.Any())
                {
                    Logger.LogInfo("Could not find records for table " + tableName+ " in the csv");
                    return;
                }

                Logger.LogInfo("Generating stage file for " + tableName);

                stgMetadata.SourceModelLabel = Constants.NotFoundString;
                stgMetadata.SourceModelValue = Constants.NotFoundString;
                stgMetadata.TableName = tableName;
                stgMetadata.HashDiff = false;
                stgMetadata.CompositeKeysPresent = false;
                stgMetadata.IsFIleTypeBR = false;

                foreach (var tableRecord in tableRecords)
                {
                    if (tableRecord.ColumnName.Equals(Constants.SrcHashDiff, StringComparison.OrdinalIgnoreCase))
                        stgMetadata.HashDiff = true;

                }
                //composite keys for hash diff columns.
                if (primaryKeys.Count > 1)
                {
                    stgMetadata.PrimaryKey =
                        primaryKeys.Single(e => e.Contains("_HK", StringComparison.OrdinalIgnoreCase));
                    stgMetadata.Compositekeys = primaryKeys.Where((key) => key != stgMetadata.PrimaryKey).ToList();
                    stgMetadata.CompositeKeysPresent = true;
                }

                var pFrom = tableName.LastIndexOf('_');
                var sourceModels = GetSourceModel(tableRecords);
                stgMetadata.SourceModelValue = tableName.Substring(pFrom + 1).ToUpperInvariant();
                stgMetadata.HashedColumns = GetHashedColumns(tableRecords);
                stgMetadata.DerivedColumns = GetDerivedColumns(tableRecords);
                stgMetadata.Columns = GetHashDiffColumns(tableRecords);
                stgMetadata.Tags = CsvParser.GetTags(csvDataSource, tableName);

                //TODO : Revisit this for lnk  we are populating ??? for now
                //if (tableName.Contains("lnk", StringComparison.OrdinalIgnoreCase))
                //{
                //    stgMetadata.SourceModelLabel = Constants.NotFoundString;
                //    stgMetadata.SourceModelValue = Constants.NotFoundString;
                //}

                outputFilePath += "stage";
                if (tableName.StartsWith(Constants.SatBrFileName, StringComparison.OrdinalIgnoreCase))
                {
                    outputFilePath += "businessrules";
                    Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                    stgMetadata.IsFIleTypeBR = true;
                }
                Utility.CreateDirectoryIfDoesNotExists(outputFilePath);
                if (sourceModels.Count > 1)
                {
                    foreach (var SourceModel in sourceModels)
                    {
                        stgMetadata.SourceModelLabel = SourceModel;
                        var stgTemplate = new StgTemplate(stgMetadata);
                        var content = stgTemplate.TransformText();
                        // Getting the last two segment of source table, seperated with an underscore
                        var fileName = SourceModel;
                        var index = fileName.LastIndexOf(".");
                        index = fileName.Substring(0, index-1).LastIndexOf(".");
                        fileName = fileName.Substring(index + 1).Replace(".", "_").ToLower();

                        var pathStr = $"{outputFilePath}\\stg_{tableName}_{fileName}.sql";
                        File.WriteAllText(pathStr, content);
                        Logger.LogInfo("Generated stage file for " + tableName);
                    }
                }
                else 
                {
                    
                    if (!string.IsNullOrWhiteSpace(sourceModels[0]))
                    {
                        stgMetadata.SourceModelLabel = sourceModels[0];
                    }
                    var stgTemplate = new StgTemplate(stgMetadata);
                    var content = stgTemplate.TransformText();

                    var pathStr = $"{outputFilePath}\\stg_{tableName}.sql";
                    File.WriteAllText(pathStr, content);
                    Logger.LogInfo("Generated stage file for " + tableName);
                }


            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorGeneratingFileForTable("STG", tableName, e.Message), "{@StgMetadata}", stgMetadata);
            }
        }

        private static List<string> GetHashDiffColumns(List<CsvDataSource> tableRecords)
        {
            List<string> hasDiffColumns = new List<string>();

            foreach (var record in tableRecords)
            {
                if (record.HashdiffColumns.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                {
                    hasDiffColumns.Add(record.ColumnName);
                }
            }

            return hasDiffColumns.Distinct().ToList();
        }


        private static List<LabelListPair> GetDerivedColumns(List<CsvDataSource> tableRecords)
        {
            List<LabelListPair> derivedColumns = new List<LabelListPair>();


            // Ignore rows containing _HK and Timestamp in the "columnname" column.
            tableRecords = tableRecords.Where(e => !e.ColumnName.Contains("_HK") && !e.ColumnName.Equals(Constants.LoadTimestamp) && !string.IsNullOrWhiteSpace(e.StageColumns)).ToList();

            List<string> distinctColumnNameList = tableRecords.Select(e => e.ColumnName).Distinct().ToList();

            foreach (var distinctColumnName in distinctColumnNameList)
            {
                LabelListPair derivedRecord = new LabelListPair();
                // Getting values from column 'StageColumns' to be mapped with column names
                derivedRecord.Label = distinctColumnName;
                List<string> columnValuesList = new List<string>();
                foreach (var record in tableRecords)
                {
                    if (record.ColumnName.Equals(distinctColumnName))
                    {
                        columnValuesList.Add(record.StageColumns);
                    }

                }

                derivedRecord.Value = columnValuesList.Distinct().ToList();
                derivedColumns.Add(derivedRecord);
            }
            foreach (var cols in derivedColumns)
            {
                if (cols.Label.Equals("RECORD_SOURCE"))
                {
                    var recordSource = "!" + cols.Value[0];
                    cols.Value = new List<string> { recordSource };
                }
                if (cols.Label.Equals("EFFECTIVE_TIMESTAMP"))
                {
                    cols.Value = new List<string> { "TO_TIMESTAMP(EFFECTIVEDATE)" };
                }
            }

            return derivedColumns;
        }

        private static List<LabelListPair> GetHashedColumns(List<CsvDataSource> tableRecords)
        {
            //Hashed Columns will only contain rows which have _HK in the "columnname" column
            var hashedColumns = new List<LabelListPair>();
            var hashedColumnRecords = tableRecords.Where(e => e.ColumnName.Contains("_HK")).ToList();
            var distinctHashedColumnNames = hashedColumnRecords.Select(e => e.ColumnName).Distinct().ToList();

            foreach (var distinctHashedColumnName in distinctHashedColumnNames)
            {
                var hashedColumnRecord = new LabelListPair { Label = distinctHashedColumnName };

                var columnValuesList = new List<string>();
                foreach (var record in hashedColumnRecords)
                {
                    if (record.ColumnName.Equals(distinctHashedColumnName))
                    {
                        //Getting hashed column values by breaking up the MD5 string if available
                        if (record.StageColumns.Contains("MD5",StringComparison.OrdinalIgnoreCase))
                        {
                            var pFrom = record.StageColumns.LastIndexOf("(", StringComparison.Ordinal) + 1;
                            var pTo = record.StageColumns.IndexOf(")", StringComparison.Ordinal);
                            columnValuesList.AddRange(record.StageColumns.Substring(pFrom, pTo - pFrom).Replace(" ", string.Empty).Split(",").ToList());
                        }
                    }
                }
                var tableName = tableRecords[0].TableName;
                if (!columnValuesList.Any())
                {
                    columnValuesList.Add(Constants.NotFoundString);
                    Logger.LogWarning($"Could not find MD5 values for the key: {distinctHashedColumnName} in SourceTransform column for table: {tableName}");
                }
                hashedColumnRecord.Value = columnValuesList.Distinct().ToList();
                hashedColumns.Add(hashedColumnRecord);
            }

            return hashedColumns;

        }

        private static List<string> GetSourceModel(List<CsvDataSource> tableRecords)
        {
            var SourceModels =tableRecords.Select(e => e.SourceModel).Distinct().ToList();
            return SourceModels;
        }

    }
}