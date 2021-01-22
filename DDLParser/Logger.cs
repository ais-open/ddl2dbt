using System;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace DDLParser
{
    public static class Logger
    {
        public static void SetupLogger()
        {
            //var outputTemplate =   "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var outputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";

            // Logger  
            //var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u4}] | {Message:l}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: outputTemplate, theme:SystemConsoleTheme.Literate)
                .WriteTo.File($"logs/log-{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fff}.log")              
                .CreateLogger();

        }
    }
}
