using System.Text;
using PoppoKoubou.CommonLibrary.Log.Domain;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UnityLogFormatter : ILogFormatter
    {
        public string Format(LogMessage message)
        {
            // リッチテキスト使用有無のチェック
            if (!GlobalLogSettings.UseRichText) return message.Message;
            // プライマリカラーが指定されていれば優先して使用、それ以外はログレベルに対応する色を使用
            string color = !string.IsNullOrEmpty(message.PrimaryColor)
                ? message.PrimaryColor
                : GetDefaultColorForLevel(message.Level);

            // 出力先が Unity の場合、<color> タグで色指定可能
            return $"<color={color}>{Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Message))}</color>";
        }

        private string GetDefaultColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose: return "#B0B0B0"; // やや薄いグレー
                case LogLevel.Debug: return "#AAAAAA"; // 中間的なグレー
                case LogLevel.Info: return "#FFFFFF"; // 白（デフォルト）
                case LogLevel.Warning: return "#FFFF00"; // 黄色
                case LogLevel.Error: return "#FF0000"; // 赤
                case LogLevel.Fatal: return "#FF00FF"; // マゼンタ
                default: return "#FFFFFF";
            }
        }
    }
}