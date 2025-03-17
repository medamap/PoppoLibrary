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
        /// <summary>ログ出力</summary>
        private readonly IPublisher<LogMessage> _logPublisher;
        /// <summary>ログを受信するためのサブスクライバー</summary>
        private readonly ISubscriber<LogMessage> _logSubscriber;
        private IDisposable _disposable;
        /// <summary>Unityログフォーマッタ</summary>
        private readonly ILogFormatter _formatter;
        
        /// <summary>依存注入</summary>
        [Inject] public UnityLogProvider(
            LogApi logApi,
            IPublisher<LogMessage> logPublisher,
            ISubscriber<LogMessage> logSubscriber,
            ILogFormatter formatter)
        {
            _logApi = logApi;
            _logPublisher = logPublisher;
            _logSubscriber = logSubscriber;
            _formatter = formatter;
            _logPublisher.Debug($"UnityLogProvider.UnityLogProvider()");
        }
        
        /// <summary>プロバイダ初期化</summary>
        public void Initialize()
        {
            _logPublisher.Debug($"UnityLogProvider.Initialize()");
            // ログを受信したらコンソールに出力する
            var disposables = DisposableBag.CreateBuilder();
            _logSubscriber.Subscribe(
                logMessage => Debug.Log(_formatter.Format(logMessage)),
                logMessage => _logApi.IsEnabledLogLevel(logMessage))
                .AddTo(disposables);
            _disposable = disposables.Build();
        }

        /// <summary>リソース解放</summary>
        public void Dispose()
        {
            _logPublisher.Debug($"UnityLogProvider.Dispose()");
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
