using System;
using MessagePack;
using MessagePack.Formatters;

namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    /// <summary>ログメッセージ</summary>
    [MessagePackObject] public struct LogMessage
    {
        /// <summary>ログ出力タイプ</summary>
        [Key(0)] public LogType Type { get; }
        /// <summary>ログレベル</summary>
        [Key(1)] public LogLevel Level { get; }
        /// <summary>ログメッセージ</summary>
        [Key(2)] public string Message { get; }
        /// <summary>オプションのプライマリカラー、設定されていれば、こちらが優先される</summary>
        [Key(3)] public string PrimaryColor { get; }

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
            => new(LogType.AddLastLine, level, message, primaryColor);

        /// <summary>最終行を置き換え（ログレベル指定可能、省略時は Info）</summary>
        public static LogMessage ReplaceLine(string message, LogLevel level = LogLevel.Info, string primaryColor = null)
            => new(LogType.ReplaceLastLine, level, message, primaryColor);
    }

    // LogMessage 用のカスタムフォーマッター
    public class LogMessageFormatter : IMessagePackFormatter<LogMessage>
    {
        public LogMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // LogMessage を 4 要素の配列としてシリアライズしている前提
            int count = reader.ReadArrayHeader();
            if (count != 4)
            {
                throw new InvalidOperationException("Invalid array length for LogMessage");
            }
            
            // LogType は enum として int でシリアライズしている前提
            int typeInt = reader.ReadInt32();
            LogType type = (LogType)typeInt;
            
            // LogLevel は enum として int でシリアライズしている前提
            int levelInt = reader.ReadInt32();
            LogLevel level = (LogLevel)levelInt;
            
            string message = reader.ReadString();
            string primaryColor = reader.ReadString();
            
            return new LogMessage(type, level, message, primaryColor);
        }

        public void Serialize(ref MessagePackWriter writer, LogMessage value, MessagePackSerializerOptions options)
        {
            // LogMessage を 4 要素の配列としてシリアライズする
            writer.WriteArrayHeader(4);
            writer.Write((int)value.Type);
            writer.Write((int)value.Level);
            writer.Write(value.Message);
            writer.Write(value.PrimaryColor);
        }
    }
}
