using System;
using MessagePipe;
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
        private readonly IPublisher<Log.Domain.LogMessage> _logPublisher;
        
        /// <summary>依存注入</summary>
        [Inject] public InteractUIDispatch(
            ISubscriber<InteractUI> interactUISubscriber,
            IPublisher<Log.Domain.LogMessage> logPublisher)
        {
            Debug.Log($"InteractUIProvider.InteractUIProvider()");
            _interactUISubscriber = interactUISubscriber;
            _logPublisher = logPublisher;
        }
        
        /// <summary>プロバイダ初期化</summary>
        public void Initialize()
        {
            Debug.Log($"CarLineOperationProvider.Initialize()");
            // InteractUI受信用サブスクライバー登録
            var disposables = DisposableBag.CreateBuilder();
            _interactUISubscriber.Subscribe(ev =>
            {
                // ログ送信
                _logPublisher.Publish(Log.Domain.LogMessage.AddLine($"InteractUI受信 ev={ev.Message}"));
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
            Debug.Log($"CarLineOperationProvider.Dispose()");
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
