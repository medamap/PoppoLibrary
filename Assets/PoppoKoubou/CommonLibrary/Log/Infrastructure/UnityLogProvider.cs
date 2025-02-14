using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    /// <summary>Unityログ処理プロバイダ</summary>
    public class UnityLogProvider : ILogProvider
    {
        /// <summary>ログを受信するためのサブスクライバー</summary>
        private readonly ISubscriber<Domain.LogMessage> _logSubscriber;
        private IDisposable _disposable;
        
        /// <summary>依存注入</summary>
        [Inject] public UnityLogProvider(ISubscriber<Domain.LogMessage> logSubscriber)
        {
            Debug.Log($"UnityLogProvider.UnityLogProvider()");
            _logSubscriber = logSubscriber;
        }
        
        /// <summary>プロバイダ初期化</summary>
        public void Initialize()
        {
            Debug.Log($"UnityLogProvider.Initialize()");
            // ログを受信したらコンソールに出力する
            var disposables = DisposableBag.CreateBuilder();
            _logSubscriber.Subscribe(x => Debug.Log(x.Message)).AddTo(disposables);
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
