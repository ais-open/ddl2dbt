using System.IO;

namespace ddl3dbt
{
    internal class Utility
    {
        public static string ErrorGeneratingFileForTable(string modelName, string tableName, string exceptionMessage)
        {
            return $"Error generating {modelName} file for table: {tableName}, Error Details: {exceptionMessage}, Please verify the logs for more details.";
        }

        public static string ErrorInTheApplication(string exceptionMessage)
        {
            return $"Error occured in the application, Error Details: {exceptionMessage}, Please verify the logs for more details.";
        }
        public static void CreateDirectoryIfDoesNotExists(string path) { if (!Directory.Exists(path)) Directory.CreateDirectory(path); }
    }
}