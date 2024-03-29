﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace ddl2dbt.Tests
{
    internal class StgFileTests : TestsBase
    {
        [Test]
        //[Ignore("waiting on requirements clarification")]
        public void GenerateModelFiles_WithAValidDDLFileForStgModelFilesGeneration_ShouldCreateStgTemplateFile()
        {
            //Arrange
            var expectedStgFileNames = new List<string> { "stg_source_model_1.sql", "stg_source_model_2.sql" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, "*", OutPutFilePath);

            //Assert
            Assert.Multiple(() =>
            {
                AssertGeneratedModelFiles(expectedStgFileNames, "stage");
            });
        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForStgModelFilesGeneration_WithoutSpecifyingModels_ShouldCreateStgTemplateFile()
        {
            //Arrange
            var expectedStgFileNames = new List<string> { "stg_source_model_1.sql", "stg_source_model_2.sql" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, string.Empty, OutPutFilePath);

            //Assert
            Assert.Multiple(() =>
            {
                AssertGeneratedModelFiles(expectedStgFileNames, "stage");
            });
        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForStgModelFilesGeneration_WithOnlyStgModels_ShouldThrowException()
        {
            Assert.Multiple(() =>
            {
                //Assert Invalid Model
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.ModelErrorMessage),
                    () => DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, "stg", OutPutFilePath));
            });
        }
    }
}