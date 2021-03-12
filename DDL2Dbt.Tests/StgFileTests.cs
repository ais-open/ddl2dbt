using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace DDL2Dbt.Tests
{
    internal class StgFileTests : TestsBase
    {
        [Test]
        //[Ignore("waiting on requirements clarification")]
        public void GenerateModelFiles_WithAValidDDLFileForStgModelFilesGeneration_ShouldCreateStgTemplateFile()
        {
            //Arrange
            var expectedStgFileNames = new List<string> { "stg_hub_coverage.sql", "stg_hub_policy_peak_policy.sql"};
            var expectedStgSatBrFilesNames = new List<string> { "stg_sat_br_coverage_ref_cov_ded_lit.sql" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, "*", OutPutFilePath);

            //Assert
            Assert.Multiple(() =>
            {
                AssertGeneratedModelFiles(expectedStgFileNames, "stage");
                AssertGeneratedModelFiles(expectedStgSatBrFilesNames, "stagebusinessrules");
            });
        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForStgModelFilesGeneration_WithoutSpecifyingModels_ShouldCreateStgTemplateFile()
        {
            //Arrange
            var expectedStgFileNames = new List<string> { "stg_hub_coverage.sql", "stg_hub_policy_peak_policy.sql" };
            var expectedStgSatBrFilesNames = new List<string> { "stg_sat_br_coverage_ref_cov_ded_lit.sql" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, string.Empty, OutPutFilePath);

            //Assert
            Assert.Multiple(() =>
            {
                AssertGeneratedModelFiles(expectedStgFileNames, "stage");
                AssertGeneratedModelFiles(expectedStgSatBrFilesNames, "stagebusinessrules");
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