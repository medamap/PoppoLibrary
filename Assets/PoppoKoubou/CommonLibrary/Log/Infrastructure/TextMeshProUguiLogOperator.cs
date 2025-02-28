using System;
using PoppoKoubou.CommonLibrary.Log.Domain;
using TMPro;
using Cysharp.Text;
using PoppoKoubou.CommonLibrary.Log.Application;
using UnityEngine.UI;
using VContainer;
using LogType = PoppoKoubou.CommonLibrary.Log.Domain.LogType;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    /// <summary>ログオペレータ</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TextMeshProUguiLogOperator : ILogOperator
    {
        /// <summary>ログAPI</summary>
        private readonly LogApi _logApi;
        /// <summary>ログフォーマッタ</summary>
        private readonly ILogFormatter _formatter;

        /// <summary>TextMeshProUGUI</summary>
        private TextMeshProUGUI _tmpText;

        /// <summary>Stringビルダー</summary>
        private Utf16ValueStringBuilder _sb;
        
        /// <summary>依存注入</summary>
        [Inject] public TextMeshProUguiLogOperator(
            LogApi logApi,
            ILogFormatter formatter)
        {
            _logApi = logApi;
            _formatter = formatter;
        }
        
        /// <summary>初期化</summary>
        public void Initialize(object logObject)
        {
            _tmpText = (TextMeshProUGUI)logObject;
            _sb = ZString.CreateStringBuilder();
            _sb.Append(_tmpText.text);
        }

        /// <summary>イベントログを処理する</summary>
        public void OnLogEvent(LogMessage ev, int overLine = 0)
        {
            // ログレベルが有効でないなら何もしない
            if (!_logApi.IsEnabledLogLevel(ev)) return;
            // ログタイプによって処理を分岐
            switch (ev.Type)
            {
                case LogType.AddLastLine: AddLastLineLog(_formatter.Format(ev), overLine); break;
                case LogType.ReplaceLastLine: ReplaceLastLineLog(_formatter.Format(ev), overLine); break;
            }
        }

        /// <summary>ログテキストを更新する</summary>
        public void AddLastLineLog(string log, int overLine = 0)
        {
            // ログがまだ空ならそのまま追加、そうでなければ改行を追加してから追加
            if (_sb.Length == 0)
            {
                _sb.Append(log);
            }
            else
            {
                _sb.AppendFormat("\n{0}",log);
            }
            _tmpText.text = _sb.ToString();

            // ログテキストがオーバーフローしてるなら、先頭行を削除する、これはオーバーフローが解消されるまで何度も削除する
            while (IsTextOverflowing(overLine))
            {
                var sbSpan = _sb.AsSpan();
                var index = sbSpan.IndexOf('\n');
                if (index >= 0)
                {
                    _sb.Remove(0, index + 1);
                }
                else
                {
                    // 1行しかない場合はループを抜ける
                    break;
                }
                _tmpText.text = _sb.ToString();
            }
        }

        /// <summary>ログテキストの最後の行を条件付きで置換する</summary>
        public void ReplaceLastLineLog(string log, int overLine = 0)
        {
            // logを空白で分割し、最初の要素を取得する
            string[] lines = log.Split(' ');
            var firstTokenOfLog = (lines.Length > 0) ? lines[0] : null;
            // 最初の要素がnullなら何もしない
            if (firstTokenOfLog == null) return;
            // ログの最後の行を取得
            var sbSpan = _sb.AsSpan();
            var index = sbSpan.LastIndexOf('\n');
            // 最後の改行が見つかったら、その後ろの文字列の開始位置を取得する
            var startIndex = (index >= 0) ? index + 1 : 0;
            // 最後の行が見つかったら
            if (index >= 0)
            {
                // 最後の行の先頭が最初の要素と一致するなら、最後の行を置き換え、そうでなければ追加
                if (sbSpan.Slice(startIndex).StartsWith(firstTokenOfLog.AsSpan()))
                {
                    _sb.Remove(startIndex, sbSpan.Length - startIndex);
                    _sb.AppendFormat("{0}", log);
                }
                else
                {
                    AddLastLineLog(log);
                }
            }
            // 最後の行が見つからなかったら追加する
            else
            {
                AddLastLineLog(log);
            }
            _tmpText.text = _sb.ToString();
        }

        /// <summary>テキストがオーバーフローしているかどうかを判定する</summary>
        private bool IsTextOverflowing(int overLine = 0)
        {
            // テキストの再構築
            _tmpText.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_tmpText.rectTransform);
            // テキストの必要な高さとRectTransformの高さを比較
            return _tmpText.preferredHeight > _tmpText.rectTransform.rect.height + overLine;
        }

        /// <summary>リソース解放</summary>
        public void Dispose()
        {
            _tmpText = null;
            _sb.Dispose();
        }
    }
}
