using System;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

using Serilog.Sinks.SystemConsole.Themes;

namespace ddl3dbt.Logging
{
    internal static class Logger
    {
        public static void SetupLogger()
        {
            const string outputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";

            const string fileOutputTemplate= "[{Timestamp:yyyy-MM-dd_HH.mm.ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(outputTemplate: outputTemplate, theme: SystemConsoleTheme.Literate, restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File($"logs/log-{DateTime.Now:yyyy-MM-dd_HH.mm.ss.fff}.log", outputTemplate: fileOutputTemplate)
                .CreateLogger();
        }

        public static void LogError(Exception ex, string message)
        {
            Log.Error(message);
            Log.Verbose(ex,string.Empty);
        }

        public static void LogError(Exception ex, string message, string messageTemplate, object obj)
        {
            Log.Error(message);
            Log.Verbose(ex,  messageTemplate, obj);
        }


        public static void LogInfo(string message)
        {
            Log.Information(message);
        }

        public static void LogInfo(string message, object template)
        {
            Log.Information(message, template);
        }

        public static void LogWarning(string message)
        {
            Log.Warning(message);
        }
        public static void LogVerbose(string message)
        {
            Log.Verbose(message);
        }
        public static void LogVerbose(Exception e,string message)
        {
            Log.Verbose(e,message);
        }
    }
}
