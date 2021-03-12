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
            var expectedLinkAndYamlFileNames = new List<string> { "lnk_file1.sql", "lnk_file1.yml", "lnk_file2.sql", "lnk_file2.yml" };
            var expectedDocFilesNames = new List<string> { "lnk_file1.docs", "lnk_file2.docs" };

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