using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ddl2dbt.Logging;
using ddl2dbt.Parsers;
using ddl2dbt.TemplateModels;
using ddl2dbt.Templates.StgTemplates;

namespace ddl2dbt.ModelFileGenerators
{
    internal class StgFileGenerator
    {
        public static void GenerateFile(List<CsvDataSource> csvDataSource, string outputFilePath)
        {
            var stgMetadata = new StgMetadata();
            
            //get unique source models
            var sourceModels = csvDataSource.Select(e => e.SourceModel).Distinct().ToList();
            //remove empty spaces if any
            sourceModels = sourceModels.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (sourceModels == null || !sourceModels.Any())
            {
                Logger.LogInfo("Could not find any valid Source Model in the csv");
                return;
            }

            outputFilePath += "stage";
            Utility.CreateDirectoryIfDoesNotExists(outputFilePath);

            foreach (var sourceModel in sourceModels)
            {
                try
                {
                    //get the records related to the source model
                    List<CsvDataSource> tableRecords = csvDataSource.Where(e => e.SourceModel.Equals(sourceModel, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (tableRecords == null || !tableRecords.Any())
                    {
                        Logger.LogInfo("Could not find records for Source Model " + sourceModel + " in the csv");
                        break;
                    }

                    Logger.LogInfo("Generating stage file for " + sourceModel);

                    if (sourceModel.Contains("."))
                    {
                        stgMetadata.SourceModelLabel = sourceModel.Substring(0,sourceModel.IndexOf("."));
                        stgMetadata.SourceModelValue = sourceModel.Substring(sourceModel.LastIndexOf(".")+1);
                    }
                    else 
                    {
                        stgMetadata.SourceModelLabel = Constants.NotFoundString;
                        stgMetadata.SourceModelValue = Constants.NotFoundString;
                    }
                    
                    stgMetadata.HashDiff = false;
                    stgMetadata.IsFIleTypeBR = false;

                    foreach (var tableRecord in tableRecords)
                    {
                        if (tableRecord.ColumnName.Equals(Constants.SrcHashDiff, StringComparison.OrdinalIgnoreCase))
                            stgMetadata.HashDiff = true;

                    }

                    stgMetadata.HashedColumns = GetHashedColumns(tableRecords);
                    stgMetadata.DerivedColumns = GetDerivedColumns(tableRecords);
                    stgMetadata.Columns = GetHashDiffColumns(tableRecords);
                    stgMetadata.Tags = GetTags(tableRecords, sourceModel).Select(i => i.ToString()).ToArray();

                    // getting the stage file name
                    var fileName = sourceModel;
                    if (fileName.Contains("."))
                    {
                        var index = fileName.LastIndexOf(".");
                        index = fileName.Substring(0, index - 1).LastIndexOf(".");
                        if (index != -1)
                        {
                            fileName = fileName.Substring(index + 1).Replace(".", "_").ToLower();
                        }
                    }
                    
                    var stgTemplate = new StgTemplate(stgMetadata);
                    var content = stgTemplate.TransformText();

                    var pathStr = $"{outputFilePath}\\stg_{fileName}.sql";
                    File.WriteAllText(pathStr, content);
                    Logger.LogInfo("Generated stage file for Soure Model: " + sourceModel);
                    
                }
                catch (Exception e)
                {
                    Logger.LogError(e, Utility.ErrorGeneratingFileForSourceModel("STG", sourceModel ,e.Message), "{@StgMetadata}", stgMetadata);
                }

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
            if (hasDiffColumns == null || !hasDiffColumns.Any())
            {
                hasDiffColumns = new List<string> { Constants.NotFoundString };
            }
            return hasDiffColumns.Distinct().ToList();
        }

        private static List<string> GetTags(List<CsvDataSource> tableRecords, string sourceModel)
        {
            var tags = new List<string>();
            foreach (var tableRecord in tableRecords) 
            {
                var tag = tableRecord.Tags;
                tags.AddRange(tag.Split(",").ToList());
            }
            //remove empty spaces if any
            tags = tags.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (tags == null || !tags.Any())
            {
                Logger.LogWarning("Could not find tags in the csv file for Source Model: "+ sourceModel);
                tags = new List<string> { Constants.NotFoundString };
            }
            return tags;
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
                    var recordSource = "!" + cols.Value[0].Replace("'", string.Empty);
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


    }
}