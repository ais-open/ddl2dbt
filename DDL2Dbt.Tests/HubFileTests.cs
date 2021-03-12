using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DDL2Dbt.Tests
{
    internal class HubFileTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForHubModelFilesGeneration_ShouldCreateHubTemplateFileDocsFileAndYamlFile()
        {
            //Arrange
            var expectedHubAndYamlFileNames = new List<string> { "hub_policy.sql", "hub_policy.yml", "hub_vehicle.sql", "hub_vehicle.yml" };
            var expectedDocFilesNames = new List<string> { "hub_policy.docs", "hub_vehicle.docs" };

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, Constants.HubFileName, OutPutFilePath);
        
            //Assert
            AssertGeneratedModelFiles(expectedHubAndYamlFileNames, expectedDocFilesNames, "hubs");
        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFile_And_WithoutCSV_ForHubModelFilesGeneration_ShouldCreateHubTemplateFiles_WithQuestionsMarksInTheTags()
        {
            //Arrange
            // - done in the setup

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, string.Empty, Constants.HubFileName, OutPutFilePath);

            var expectedHubFileNameList = GetFileNamesList(OutPutFilePath + "hubs");

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(expectedHubFileNameList, "Hub files are not generated");

                foreach (var fileName in expectedHubFileNameList.Where(e => e.EndsWith(".sql")))
                {
                    StringAssert.Contains("{{ config(tags = ['???']) }}",
                        File.ReadAllText(OutPutFilePath + @"hubs\" + fileName));
                }
            });
        }
    }
}