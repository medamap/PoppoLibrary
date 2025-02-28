using PoppoKoubou.CommonLibrary.Log.Domain;

// ReSharper disable ClassNeverInstantiated.Global

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    public class LogSettings : ILogSettings, IReadOnlyLogSettings
    {
        public LogLevel LogLevel { get; private set; } = GlobalLogSettings.LogLevel == LogLevel.None
             ? LogLevel.Info | LogLevel.Warning | LogLevel.Error | LogLevel.Fatal
             : GlobalLogSettings.LogLevel;
        public void UpdateLogSettings(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }
    }
}