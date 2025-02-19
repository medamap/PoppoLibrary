using System;

namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    /// <summary>ログオペレータ</summary>
    public interface ILogOperator : IDisposable
    {
        /// <summary>初期化</summary>
        public void Initialize(object logObject);
        /// <summary>ログイベントを処理する</summary>
        public void OnLogEvent(LogMessage ev, int overLine = 0);
        /// <summary>ログテキストを更新する</summary>
        public void AddLastLineLog(string log, int overLine = 0);
        /// <summary>ログテキストの最後の行を条件付きで置換する</summary>
        public void ReplaceLastLineLog(string log, int overLine = 0);
    }
}