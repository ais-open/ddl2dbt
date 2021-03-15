using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace DDL2Dbt.Tests
{
    internal class TestsBase
    {
        protected string CurrentProjectDirectoryPath;
        protected string DDLFilePath;
        protected string InvalidDDLFilePath;
        protected string CsvFilePath;
        protected string OutPutFilePath;
        protected string ActualFilesPath;

        [SetUp]
        protected void Setup()
        {
            CurrentProjectDirectoryPath = Directory.GetParent(Environment.CurrentDirectory).Parent?.Parent?.Parent?.FullName;
            DDLFilePath = CurrentProjectDirectoryPath + @"\DDL2Dbt.Tests\ActualFiles\DDL.ddl";
            InvalidDDLFilePath = CurrentProjectDirectoryPath + @"\DDL2Dbt.Tests\ActualFiles123\DDL.ddl";
            CsvFilePath = CurrentProjectDirectoryPath + @"\DDL2Dbt.Tests\ActualFiles\csvFile.csv";
            OutPutFilePath = CurrentProjectDirectoryPath + @"\DDL2Dbt.Tests\ExpectedFiles\";
            ActualFilesPath = CurrentProjectDirectoryPath + @"\DDL2Dbt.Tests\ActualFiles\";
        }

        protected void AssertGeneratedModelFiles(List<string> expectedModelAndYamlFileNames,
            List<string> expectedDocFilesNames, string generatedFilesFolderName)
        {
            //Assert
            Assert.Multiple(() =>
            {
                AssertModelFiles(expectedModelAndYamlFileNames, generatedFilesFolderName);
                AssertDocFiles(expectedDocFilesNames, generatedFilesFolderName);
            });
        }

        protected void AssertGeneratedModelFiles(List<string> expectedModelAndYamlFileNames,
         string generatedFilesFolderName)
        {
            //Assert
            Assert.Multiple(() =>
            {
                AssertModelFiles(expectedModelAndYamlFileNames, generatedFilesFolderName);
            });
        }

        private void AssertModelFiles(List<string> expectedModelAndYamlFileNames, string generatedFilesFolderName)
        {
            Assert.Multiple(() =>
            {
                var expectedFilesPath = OutPutFilePath + generatedFilesFolderName + "\\";
                var generatedModelAndYamlFileNamesList = GetFileNamesList(expectedFilesPath);


                Assert.IsNotEmpty(generatedModelAndYamlFileNamesList, "Hub and Yml files are not generated.");

                foreach (var expectedFileName in expectedModelAndYamlFileNames)
                {
                    Assert.Contains(expectedFileName, generatedModelAndYamlFileNamesList);
                    var expected = expectedFilesPath + expectedFileName;
                    var actual = ActualFilesPath + generatedFilesFolderName + "\\" + expectedFileName;


                    FileAssert.Exists(expected, $"{expected} does not exist");
                    FileAssert.Exists(actual, $"{actual} does not exist");
                    TestContext.WriteLine(expected + " ---vs--- " + actual);

                    FileAssert.AreEqual(expected, actual,
                        "Test failure for " + expectedFileName);
                }
            });
        }

        private void AssertDocFiles(List<string> expectedDocFilesNames, string generatedFilesFolderName)
        {
            Assert.Multiple(() =>
            {
                var docFilesPath = OutPutFilePath + "docs\\" + generatedFilesFolderName + "\\";
                var generatedDocFileNameList = GetFileNamesList(docFilesPath);
                foreach (var expectedFileName in expectedDocFilesNames)
                {
                    Assert.Contains(expectedFileName, generatedDocFileNameList);
                    var expected = docFilesPath + expectedFileName;
                    var actual = ActualFilesPath + "docs\\" + generatedFilesFolderName + "\\" + expectedFileName;

                    FileAssert.Exists(expected, $"{expected} does not exist");
                    FileAssert.Exists(actual, $"{actual} does not exist");
                    TestContext.WriteLine(expected + " ---vs--- " + actual);


                    FileAssert.AreEqual(expected, actual,
                        "Test failure for " + expectedFileName);
                }
            });
        }

        protected static List<string> GetFileNamesList(string path)
        {
            DirectoryInfo actualDirectoryPath =
                new DirectoryInfo(path);
            FileInfo[] actualFiles = actualDirectoryPath.GetFiles("*.*");
            List<string> fileNamesList = new List<string>();
            foreach (FileInfo file in actualFiles)
            {
                fileNamesList.Add(file.Name);
            }
            return fileNamesList;
        }

        [TearDown]
        protected void CleanUp()
        {
            
                if (Directory.Exists(OutPutFilePath))
                    Directory.Delete(OutPutFilePath, true);


        }
    }
}