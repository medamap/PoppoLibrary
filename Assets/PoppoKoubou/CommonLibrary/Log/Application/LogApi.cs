using PoppoKoubou.CommonLibrary.Log.Domain;

// ReSharper disable ClassNeverInstantiated.Global

namespace PoppoKoubou.CommonLibrary.Log.Application
{
    public class LogApi
    {
        /// <summary>ログ設定</summary>
        private readonly ILogSettings _logSettings;
        /// <summary>依存注入</summary>
        public LogApi(ILogSettings logSettings)
        {
            _logSettings = logSettings;
        }
        /// <summary>ログレベル取得</summary>
        public LogLevel GetLogLevel() => _logSettings.LogLevel;
        /// <summary>ログレベル更新</summary>
        public void UpdateLogLevel(LogLevel logLevel) => _logSettings.UpdateLogSettings(logLevel);
        /// <summary>ログレベルが有効か</summary>
        public bool IsEnabledLogLevel(LogMessage logMessage) => (_logSettings.LogLevel & logMessage.Level) > 0;
    }
}