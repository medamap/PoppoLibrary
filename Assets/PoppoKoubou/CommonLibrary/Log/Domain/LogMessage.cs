// ReSharper disable MemberCanBePrivate.Global
namespace PoppoKoubou.CommonLibrary.Log.Domain

{
    /// <summary>ログメッセージ</summary>
    public struct LogMessage
    {
        /// <summary>ログ出力タイプ</summary>
        public LogType Type { get; }
        /// <summary>ログレベル</summary>
        public LogLevel Level { get; }
        /// <summary>ログメッセージ</summary>
        public string Message { get; }
        /// <summary>オプションのプライマリカラー、設定されていれば、こちらが優先される</summary>
        public string PrimaryColor { get; }
        /// <summary>コンストラクタ</summary>
        public LogMessage(LogType type, LogLevel level, string message, string primaryColor = null)
        {
            Type = type;
            Level = level;
            Message = message;
            PrimaryColor = primaryColor;
        }
        /// <summary>最終行に追加（ログレベル指定可能、省略時は Info）</summary>
        public static LogMessage AddLine(string message, LogLevel level = LogLevel.Info, string primaryColor = null)
            => new (LogType.AddLastLine, level, message, primaryColor);

        /// <summary>最終行を置き換え（ログレベル指定可能、省略時は Info）</summary>
        public static LogMessage ReplaceLine(string message, LogLevel level = LogLevel.Info, string primaryColor = null)
            => new (LogType.ReplaceLastLine, level, message, primaryColor);
    }
}
