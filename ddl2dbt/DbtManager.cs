﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ddl2dbt.Logging;
using ddl2dbt.ModelFileGenerators;
using ddl2dbt.Parsers;
using Serilog;

namespace ddl2dbt
{
    internal class DbtManager
    {

        public static void GenerateModelFiles(string ddl, string csv, string models, string outputPath)
        {
           
                if (string.IsNullOrWhiteSpace(models))
                models = "*";
            if (!outputPath.EndsWith("\\"))
                outputPath += "\\";

            Logger.LogInfo($"DDL file path: {ddl}");
            Logger.LogInfo($"CSV file path: {csv}");
            Logger.LogInfo($"Models: {models}");
            Logger.LogInfo($"Output file path: {outputPath}");

            var modelsArray = models.Split(',');

            if ((modelsArray.Length == 1) && string.Equals(modelsArray[0], "stg", StringComparison.OrdinalIgnoreCase))
            {
                //Logger.LogError(null, Constants.ModelErrorMessage);
                throw new Exception(Constants.ModelErrorMessage);
                //return;
            }

            if (Array.Exists(modelsArray, element =>
                string.Equals(element, Constants.HubFileName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(element, Constants.SatFileName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(element, Constants.LnkFileName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)
            ))
            {
                GenerateModels(ddl, csv, models, outputPath);
            }

            else
            {
                //Logger.LogError(null, Constants.ModelErrorMessage);
                throw new Exception(Constants.ModelErrorMessage);

            }


        }

        private static void GenerateModels(string ddlFilePath, string csvFilePath, string models, string outputFilePath)
        {
           var sqlStatements = DDLParser.BuildDdlStatementsCollection(ddlFilePath);
           var records = CsvParser.ParseCsv(csvFilePath, models);

           var fileNameArr = models.Split(',');

           foreach (var sqlStatement in sqlStatements.Where(sqlStatement => !string.IsNullOrWhiteSpace(sqlStatement)))
           {
               if (Array.Exists(fileNameArr, element => string.Equals(element, Constants.HubFileName, StringComparison.OrdinalIgnoreCase) ||
                                                        string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                   if (sqlStatement.Contains("CREATE TABLE HUB", StringComparison.OrdinalIgnoreCase))
                   {
                       var hubTableMetadata = HubFileGenerator.GenerateFile(sqlStatement, sqlStatements, outputFilePath, records);
                       YamlFileGenerator.GenerateFile(sqlStatement, outputFilePath, hubTableMetadata.TableName, records);
                       DocFileGenerator.GenerateFile(outputFilePath, hubTableMetadata.TableName);
                   }

               if (Array.Exists(fileNameArr, element => string.Equals(element, Constants.LnkFileName, StringComparison.OrdinalIgnoreCase) ||
                                                        string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                   if (sqlStatement.Contains("CREATE TABLE LNK", StringComparison.OrdinalIgnoreCase))
                   {
                       var linkTableMetadata = LinkFileGenerator.GenerateFile(sqlStatement, sqlStatements, outputFilePath, records);
                       YamlFileGenerator.GenerateFile(sqlStatement, outputFilePath, linkTableMetadata.TableName, records);
                       DocFileGenerator.GenerateFile(outputFilePath, linkTableMetadata.TableName);
                   }

               if (Array.Exists(fileNameArr, element => string.Equals(element, Constants.SatFileName, StringComparison.OrdinalIgnoreCase) ||
                                                        string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
                   if (sqlStatement.Contains("CREATE TABLE SAT", StringComparison.OrdinalIgnoreCase) || sqlStatement.Contains("CREATE TABLE MAS", StringComparison.OrdinalIgnoreCase))
                   {
                       var satTableMetadata = SatFileGenerator.GenerateFile(sqlStatement, sqlStatements, outputFilePath, records);
                       YamlFileGenerator.GenerateFile(sqlStatement, outputFilePath, satTableMetadata.TableName, records);
                       DocFileGenerator.GenerateFile(outputFilePath, satTableMetadata.TableName);
                       //GenerateStgFile(outputFilePath, fileNameArr, records, satTableMetadata.TableName, satTableMetadata.PrimaryKeys);
                   }
           }

            if (records != null && Array.Exists(fileNameArr,
                 element => string.Equals(element, "stg", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(element, "*", StringComparison.OrdinalIgnoreCase)))
            {
                StgFileGenerator.GenerateFile( records, outputFilePath);
            }

        }
    }
}