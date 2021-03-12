using System;
using NUnit.Framework;

namespace DDL2Dbt.Tests
{
    internal class GenerateModelFileValidationTests : TestsBase
    {
        [Test]
        public void GenerateModelFiles_WithAInvalidDDLFileForModelFilesGeneration_ShouldThrowException()
        {
            Assert.Multiple(() =>
            {
                //Assert Empty String
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.InvalidDDLFileExensionError),
                    () => DbtManager.GenerateModelFiles(string.Empty, string.Empty, "*", OutPutFilePath));
                //Assert Invalid Extension
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.InvalidDDLFileExensionError),
                    () => DbtManager.GenerateModelFiles(".yml", string.Empty, "*", OutPutFilePath));
                //Assert Invalid File
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.InvalidDDLFileExensionError),
                    () => DbtManager.GenerateModelFiles(CsvFilePath, string.Empty, "*", OutPutFilePath));
                //Assert Invalid File Location
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.InvalidDDLFileLocation),
                    () => DbtManager.GenerateModelFiles(InvalidDDLFilePath, CsvFilePath, "*", OutPutFilePath));
            });
        }

        [Test]
        public void GenerateModelFiles_WithAInvalidCSVFileForModelFilesGeneration_ShouldThrowException()
        {
            Assert.Multiple(() =>
            {
                //Assert Invalid File
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.InvalidCSVFileError),
                    () => DbtManager.GenerateModelFiles(DDLFilePath, DDLFilePath, "*", OutPutFilePath));
            });
        }

        [Test]
        public void GenerateModelFiles_WithAInvalidModel_ShouldThrowException()
        {
            Assert.Multiple(() =>
            {
                //Assert Invalid File
                Assert.Throws(Is.TypeOf<Exception>().And.Message.Contains(Constants.ModelErrorMessage),
                    () => DbtManager.GenerateModelFiles(DDLFilePath, DDLFilePath, "abc", OutPutFilePath));
            });
        }
    }
}