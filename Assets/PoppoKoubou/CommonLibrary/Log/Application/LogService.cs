using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou.CommonLibrary.Log.Application
{
    /// <summary>ログサービス</summary>
    public class LogService : ServiceNode
    {
        /// <summary>ログプロバイダ</summary>
        private readonly ILogProvider _logProvider;
        
        /// <summary>依存注入</summary>
        [Inject] public LogService(
            ILogProvider logProvider,
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
            : base(
                "ログサービス",
                0,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"LogService.LogService()");
            _logProvider = logProvider;
        }
        
        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            // ログプロバイダ初期化
            _logProvider.Initialize();
            LogPublisher.Publish(LogMessage.AddLine($"LogService.StartInitialize()", LogLevel.Debug));
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Publish(LogMessage.AddLine($"LogService.StartService()", LogLevel.Debug));
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.Publish(LogMessage.AddLine($"LogService.Dispose()", LogLevel.Debug));
            _logProvider?.Dispose();
        }
    }
}
