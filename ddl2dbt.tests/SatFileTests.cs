using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ddl2dbt.Tests
{
    internal class SatFileTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAValidDDLFileForSatModelFilesGeneration_ShouldCreateSatTemplateFiles()
        {
            //Arrange
            var expectedSatAndYamlFileNames = new List<string> { "sat_cust_nation_details.sql", "sat_cust_nation_details.yml" };
            //var expectedSatBrAndYmlFilesNames = new List<string>
            //{ "sat_br_file1.sql",
            //    "sat_br_file1.yml",
            //    "sat_br_file2.sql",
            //    "sat_br_file2.yml"
            //};
            var expectedSatDocFilesNames = new List<string> { "sat_cust_nation_details.docs" };
            //var expectedSatBrDocFilesNames = new List<string> { "sat_br_file1.docs", "sat_br_file2.docs" };


            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, CsvFilePath, Constants.SatFileName, OutPutFilePath);


            //Assert
            Assert.Multiple(() =>
            {
                AssertGeneratedModelFiles(expectedSatAndYamlFileNames, expectedSatDocFilesNames, "satellites");
                //AssertGeneratedModelFiles(expectedSatBrAndYmlFilesNames, expectedSatBrDocFilesNames, "satellitebusinessrules");
            });



        }

        [Test]
        public void GenerateModelFiles_WithAValidDDLFile_And_WithoutCSV_ForSatModelFilesGeneration_ShouldCreateSatTemplateFiles_WithQuestionsMarksInTheTags()
        {
            //Arrange
            // - done in the setup

            //Act
            DbtManager.GenerateModelFiles(DDLFilePath, string.Empty, Constants.SatFileName, OutPutFilePath);

            var expectedSatFileNameList = GetFileNamesList(OutPutFilePath + "satellites");
            //var expectedSatBrFileNameList = GetFileNamesList(OutPutFilePath + "satellitebusinessrules");

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(expectedSatFileNameList, "Sat files are not generated");

                foreach (var fileName in expectedSatFileNameList.Where(e => e.EndsWith(".sql")))
                {
                    StringAssert.Contains("{{ config(tags = ['???']) }}",
                        File.ReadAllText(OutPutFilePath + @"satellites\" + fileName));
                }
            });
            //Assert.Multiple(() =>
            //{
            //    Assert.IsNotEmpty(expectedSatBrFileNameList, "Sat Br files are not generated");

            //    foreach (var fileName in expectedSatBrFileNameList.Where(e => e.EndsWith(".sql")))
            //    {
            //        StringAssert.Contains("{{ config(tags = ['???']) }}",
            //            File.ReadAllText(OutPutFilePath + @"satellitebusinessrules\" + fileName));
            //    }
            //});
        }

    }
}