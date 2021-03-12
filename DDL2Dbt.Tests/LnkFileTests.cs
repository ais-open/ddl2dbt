using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DDL2Dbt.Tests
{
    internal class LnkFileTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAValidDDLAndCsvForLnkModelFilesGeneration_ShouldCreateLnkTemplateFiles()
        {
            //Arrange
            var expectedLinkAndYamlFileNames = new List<string> { "lnk_policy_transaction_has_vehicle_coverage.sql", "lnk_policy_transaction_has_vehicle_coverage.yml", "lnk_policy_has_policy_transaction.sql", "lnk_policy_has_policy_transaction.yml" };
            var expectedDocFilesNames = new List<string> { "lnk_policy_transaction_has_vehicle_coverage.docs", "lnk_policy_has_policy_transaction.docs" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, Constants.LnkFileName, OutPutFilePath);
            
            //Assert
            AssertGeneratedModelFiles(expectedLinkAndYamlFileNames, expectedDocFilesNames, "links");
        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFile_And_WithoutCSV_ForLnkModelFilesGeneration_ShouldCreateLnkTemplateFiles_WithQuestionsMarksInTheTags()
        {
            //Arrange
            // - done in the setup

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, string.Empty, Constants.LnkFileName, OutPutFilePath);

            var expectedLnkFileNameList = GetFileNamesList(OutPutFilePath + "links");

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(expectedLnkFileNameList, "Lnk files are not generated");

                foreach (var fileName in expectedLnkFileNameList.Where(e=>e.EndsWith(".sql")))
                {
                    StringAssert.Contains("{{ config(tags = ['???']) }}",
                        File.ReadAllText(OutPutFilePath + @"links\" + fileName));
                }
            });
        }
    }
}