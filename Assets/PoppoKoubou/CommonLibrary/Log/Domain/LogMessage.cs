namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    /// <summary>ログメッセージ</summary>
    public struct LogMessage
    {
        /// <summary>ログ出力タイプ</summary>
        public LogType Type { get; }
        /// <summary>ログメッセージ</summary>
        public string Message { get; }
        /// <summary>コンストラクタ</summary>
        public LogMessage(LogType type, string message)
        {
            Type = type;
            Message = message;
        }
        /// <summary>最終行に追加</summary>
        static public LogMessage AddLine(string message) => new LogMessage(LogType.AddLastLine, message);
        /// <summary>最終行を置き換え</summary>
        static public LogMessage ReplaceLine(string message) => new LogMessage(LogType.ReplaceLastLine, message);
    }
}
