using System;
using CoreWUS;
using Serilog;
using System.Runtime.CompilerServices;
using System.IO;

namespace WusClient
{
    public sealed class Logger : CoreWUS.ILogger
    {
        private readonly LogLevel _minimumLevel;

        public Logger(LogLevel minimumLevel)
        {
            _minimumLevel = minimumLevel;

            var level = new Serilog.Core.LoggingLevelSwitch();

            Serilog.Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .MinimumLevel.ControlledBy(level)
                        .CreateLogger();

            level.MinimumLevel = _minimumLevel switch
            {
                LogLevel.Verbose => Serilog.Events.LogEventLevel.Verbose,
                LogLevel.Debug => Serilog.Events.LogEventLevel.Debug,
                LogLevel.Info => Serilog.Events.LogEventLevel.Information,
                LogLevel.Warning => Serilog.Events.LogEventLevel.Warning,
                LogLevel.Error => Serilog.Events.LogEventLevel.Error,
                LogLevel.Fatal => Serilog.Events.LogEventLevel.Fatal,
                _ => Serilog.Events.LogEventLevel.Information,
            };
        }

        public void Log(LogLevel level, string message, Exception ex = null,
                        [CallerMemberName] string memberName = null,
                        [CallerFilePath] string sourceFilePath = null)
        {
            if (level < _minimumLevel)
            {
                return;
            }

            switch (level)
            {
                case LogLevel.Verbose:
                    Serilog.Log.Verbose(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
                case LogLevel.Debug:
                    Serilog.Log.Debug(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
                case LogLevel.Info:
                    Serilog.Log.Information(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
                case LogLevel.Warning:
                    Serilog.Log.Warning(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
                case LogLevel.Error:
                    Serilog.Log.Error(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
                case LogLevel.Fatal:
                    Serilog.Log.Fatal(FormatMessage(message, ex, memberName, sourceFilePath));
                    break;
            }
        }

        private static string FormatMessage(string message, Exception ex,
                                        string memberName, string sourceFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            return $"{fileName} [{memberName}] {message} {(ex != null ? ":" + ex.ToString() : "")}";
        }
    }
}