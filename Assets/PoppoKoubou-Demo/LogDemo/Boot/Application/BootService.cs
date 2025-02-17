using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;

namespace PoppoKoubou_Demo.LogDemo.Boot.Application
{
    /// <summary>ブートサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BootService : ServiceNode
    {

        /// <summary>依存注入</summary>
        public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher)
            : base(
                "LogDemo ブートサービス",
                100,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"BootService.BootService()");
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"BootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogAddLine($"BootService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogAddLine($"BootService.Dispose()", ServiceLogColor);
        }
    }
}
