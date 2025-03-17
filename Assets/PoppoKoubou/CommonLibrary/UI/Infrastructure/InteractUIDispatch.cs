using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Infrastructure
{
    /// <summary>UIインタラクトディスパッチ</summary>
    public class InteractUIDispatch : IInteractUIDispatch
    {
        /// <summary>InteractUI受診用サブスクライバー</summary>
        private readonly ISubscriber<InteractUI> _interactUISubscriber;
        private IDisposable _disposable;

        /// <summary>ログメッセージ送信用Publisher</summary>
        private readonly IPublisher<LogMessage> _logPublisher;
        
        /// <summary>依存注入</summary>
        [Inject] public InteractUIDispatch(
            ISubscriber<InteractUI> interactUISubscriber,
            IPublisher<LogMessage> logPublisher)
        {
            _interactUISubscriber = interactUISubscriber;
            _logPublisher = logPublisher;
            _logPublisher.Debug($"InteractUIProvider.InteractUIProvider()");
        }
        
        /// <summary>プロバイダ初期化</summary>
        public void Initialize()
        {
            _logPublisher.Debug($"CarLineOperationProvider.Initialize()");
            // InteractUI受信用サブスクライバー登録
            var disposables = DisposableBag.CreateBuilder();
            _interactUISubscriber.Subscribe(ev =>
            {
                // ログ送信
                _logPublisher.Debug($"InteractUI受信 ev={ev.Message}");
                // メッセージをカンマで分割
                var message = ev.Message.Split(',');
                switch (message[0])
                {
                    // メッセージの先頭が"????"の場合
                    case "????":
                    {
                        // 第二メッセージを取得
                        var name = message[1];
                        // ToDo : 適宜処理する
                        return;
                    }
                }
            }).AddTo(disposables);
            _disposable = disposables.Build();
        }

        /// <summary>リソース解放</summary>
        public void Dispose()
        {
            _logPublisher.Debug($"CarLineOperationProvider.Dispose()");
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
