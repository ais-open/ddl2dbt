using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ddl2dbt.Tests
{
    internal class HubFileTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForHubModelFilesGeneration_ShouldCreateHubTemplateFileDocsFileAndYamlFile()
        {
            //Arrange
            var expectedHubAndYamlFileNames = new List<string> { "hub_customer.sql", "hub_customer.yml" };
            var expectedDocFilesNames = new List<string> { "hub_customer.docs" };

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
                    StringAssert.Contains("{{ config(tags = ['???'])}}",
                        File.ReadAllText(OutPutFilePath + @"hubs\" + fileName));
                }
            });
        }
    }
}