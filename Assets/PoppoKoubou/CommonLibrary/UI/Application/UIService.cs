using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.UI.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.UI.Application
{
    /// <summary>UIサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIService : ServiceNode
    {
        /// <summary>UIインタラクトディスパッチ</summary>
        // ReSharper disable once InconsistentNaming
        private readonly IInteractUIDispatch _interactUIDispatch;
    
        /// <summary>依存注入</summary>
        [Inject] public UIService(
            IInteractUIDispatch interactUIDispatch,
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
            : base(
                "UIサービス",
                0,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"UIService.UIService()");
            _interactUIDispatch = interactUIDispatch;
        }
        
        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.AddLine($"UIService.StartInitialize()", LogLevel.Debug, ServiceLogColor);
            // UIインタラクトディスパッチ初期化
            _interactUIDispatch.Initialize();
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.AddLine($"UIService.StartService()", LogLevel.Debug, ServiceLogColor);
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.AddLine($"UIService.Dispose()", LogLevel.Debug, ServiceLogColor);
            _interactUIDispatch?.Dispose();
        }
    }
}