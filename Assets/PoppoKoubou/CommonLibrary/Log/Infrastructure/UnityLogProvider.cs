using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    /// <summary>Unityログ処理プロバイダ</summary>
    public class UnityLogProvider : ILogProvider
    {
        /// <summary>ログAPI</summary>
        private readonly LogApi _logApi;
        /// <summary>ログを受信するためのサブスクライバー</summary>
        private readonly ISubscriber<LogMessage> _logSubscriber;
        private IDisposable _disposable;
        /// <summary>Unityログフォーマッタ</summary>
        private readonly ILogFormatter _formatter;
        
        /// <summary>依存注入</summary>
        [Inject] public UnityLogProvider(
            LogApi logApi,
            ISubscriber<LogMessage> logSubscriber,
            ILogFormatter formatter)
        {
            Debug.Log($"UnityLogProvider.UnityLogProvider()");
            _logApi = logApi;
            _logSubscriber = logSubscriber;
            _formatter = formatter;
        }
        
        /// <summary>プロバイダ初期化</summary>
        public void Initialize()
        {
            Debug.Log($"UnityLogProvider.Initialize()");
            // ログを受信したらコンソールに出力する
            var disposables = DisposableBag.CreateBuilder();
            _logSubscriber.Subscribe(
                x => Debug.Log(_formatter.Format(x)),
                x => _logApi.IsEnabledLogLevel(x)).AddTo(disposables);
            _disposable = disposables.Build();
        }

        /// <summary>リソース解放</summary>
        public void Dispose()
        {
            Debug.Log($"UnityLogProvider.Dispose()");
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
