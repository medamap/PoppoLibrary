using System;
using PoppoKoubou.CommonLibrary.Log.Domain;
using MessagePipe;
using TMPro;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.Log.Presentation
{
    /// <summary>ログプレゼンター</summary>
    public class LogPresenter : MonoBehaviour
    {
        /// <summary>TextMeshProUGUI</summary>
        private TextMeshProUGUI _tmpText;

        /// <summary>ログを受信するサブスクライバー</summary>
        private ISubscriber<LogMessage> _logSubscriber;
        private IDisposable _disposable;
        
        /// <summary>ログオペレータ</summary>
        private ILogOperator _logOperator;

        /// <summary>依存注入</summary>
        [Inject] public void Cunstruct(
            ISubscriber<LogMessage> logSubscriber,
            ILogOperator logOperator)
        {
            _logSubscriber = logSubscriber;
            _logOperator = logOperator;
        }

        /// <summary>コンポーネントの初期化</summary>
        private void Start()
        {
            // Textコンポーネント取得
            _tmpText = GetComponent<TextMeshProUGUI>();
            // ログオペレータ初期化
            _logOperator.Initialize(_tmpText);
            // ログを受信するサブスクライバーを登録
            var disposables = DisposableBag.CreateBuilder();
            _logSubscriber.Subscribe(ev => _logOperator.OnLogEvent(ev, 512)).AddTo(disposables);
            _disposable = disposables.Build();
        }

        /// <summary>リソース解放</summary>
        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
            _logOperator?.Dispose();
            _logOperator = null;
        }
    }
}