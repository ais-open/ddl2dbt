using System;
using Serilog;

//using Microsoft.Extensions.Logging.Console;
namespace DDLParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Logger.SetupLogger();

                //TODO: Remove the argumentDetails Tuple and Create an object for capturing command line arguments.
                var (ddlFilePath, csvFilePath, fileNames, outputFilePath) = GetCommandlineArgs(args);
                Parser.ParseDDL(ddlFilePath, csvFilePath, fileNames, outputFilePath);
                Log.Information("Files generation completed.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error occured in the application");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        private static (string, string, string, string) GetCommandlineArgs(string[] args)
        {
            string ddLfilepath = "", fileNames = "*", outputFilePath = "", csvfilepath = "";
            if (args.Length > 0)
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("-ddl"))
                    {
                        ddLfilepath = args[i + 1];
                        Console.WriteLine(ddLfilepath);
                        i += 1; ;
                    }
                    if (args[i].Equals("-csv"))
                    {
                        csvfilepath = args[i + 1];
                        Console.WriteLine(csvfilepath);
                        i += 1;
                    }
                    if (args[i].Equals("-m"))
                    {
                        fileNames = args[i + 1];
                        Console.WriteLine(fileNames);
                        i += 1;
                    }
                    if (args[i].Equals("-o"))
                    {
                        outputFilePath = args[i + 1];
                        Console.WriteLine(outputFilePath);
                        i += 1;
                    }
                }


            //ParseDDL(filepath, fileNames, outputFilePath);
            return (ddLfilepath, csvfilepath, fileNames, outputFilePath);
        }
    }
}