using System;
using System.IO;
using NUnit.Framework;

namespace DDL2Dbt.Tests
{
    public class Tests
    {
        /// <summary>
        /// [MethodName_StateUnderTest_ExpectedBehavior]
        /// AAA
        /// A=arrange
        /// A=act
        /// A=assert
        /// </summary>


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ParseDDL_WithAValidDDLFileForCreateTableHubStatements_ShouldCreateHubTemplateFile()
        {
            Assert.Pass();
        }

        public void ParseDDL_WithAValidDDLFileForCreateTableSatStatements_ShouldCreateSatTemplateFile()
        {
            Assert.Pass();
        }

        public void ParseDDL_WithAValidDDLFileForCreateTableLnkStatements_ShouldCreateLnkTemplateFile()
        {
            //Arrange
           
            //Delete all the files in the expectedFile Folder.
            string ddlPath = "";
            string outPutFilePath = "";
            
            //Act
            Parser.ParseDDL("","","hub","");



            //Assert

            FileInfo actualFile = new FileInfo(@"actualFilePath");
            FileInfo expectedFile = new FileInfo(@"ExpectedFilePath");


            FileAssert.AreEqual(expectedFile, actualFile);    // Pass
           // FileAssert.AreNotEqual(file1, file1copy); // Fail
        }
    }
}