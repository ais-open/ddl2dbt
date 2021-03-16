using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ddl3dbt.Tests
{
    internal class LnkFileTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAValidDDLAndCsvForLnkModelFilesGeneration_ShouldCreateLnkTemplateFiles()
        {
            //Arrange
            var expectedLinkAndYamlFileNames = new List<string> { "lnk_customer_nation.sql", "lnk_customer_nation.yml" };
            var expectedDocFilesNames = new List<string> { "lnk_customer_nation.docs" };

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