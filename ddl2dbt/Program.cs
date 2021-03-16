using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.CompilerServices;
using ddl2dbt.Logging;
using Serilog;

[assembly: InternalsVisibleTo("ddl2dbt.tests")]
namespace ddl2dbt
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Logger.SetupLogger();
            try
            {
                var ddlFilePath = new Option<string>(new[] { "--ddl", "-d" }, "DDL File Path") { IsRequired = true };

                var outputFilePath =
                    new Option<string>(new[] { "--outputPath", "-o" }, "Output Directory") { IsRequired = true };

                var cmd = new RootCommand
                {
                    ddlFilePath,
                    new Option<string>(new[] {"--csv", "-c"}, "CSV File Path. if not provided only hub,sat,lnk file generation will be applicable."),
                    new Option<string>(new[] {"--models", "-m"}, " Options include: hub,sat and lnk or * (Default). Use comma to select multiple options Ex: sat,hub. To generate stg files either a * or a specific model name with stg should be included Ex: -m hub,stg"),
                    outputFilePath
                };

                //https://github.com/dotnet/command-line-api/issues/796
                cmd.Handler = CommandHandler.Create<string, string, string, string>(GenerateModelFiles);
                return cmd.Invoke(args);
            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorInTheApplication(e.Message));
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return 1;
        }


        private static void GenerateModelFiles(string ddl, string csv, string models, string outputPath)
        {
            try
            {
                DbtManager.GenerateModelFiles(ddl, csv, models, outputPath);

            }
            catch (Exception e)
            {
                Logger.LogError(e, Utility.ErrorInTheApplication(e.Message));
            }
        }


    }
}