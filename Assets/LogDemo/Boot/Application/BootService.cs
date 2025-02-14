using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using UnityEngine;

namespace LogDemo.Boot.Application
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
                "ブートサービス",
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
            Debug.Log($"BootService.StartInitialize()");
            LogPublisher.Publish(LogMessage.AddLine($"<color=#00ffff>BootService.StartInitialize()</color>"));
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            Debug.Log($"LogService.StartService()");
            LogPublisher.Publish(LogMessage.AddLine($"<color=#00ffff>BootService.StartService()</color>"));
            // 1ミリ秒待機
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }
    }
}
