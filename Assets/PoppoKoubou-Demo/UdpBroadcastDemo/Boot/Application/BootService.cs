using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.Boot.Application
{
    /// <summary>ブートサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BootService : ServiceNode
    {
        /// <summary>ネットワークサービスコンテナ</summary>
        private readonly NetworkInfoContainer _networkInfoContainer;
        /// <summary>UDPメッセージパブリッシャー</summary>
        private readonly IDistributedPublisher<string, UdpMessage> _udpPublisher;
        /// <summary>UDPメッセージパブリッシャー</summary>
        private readonly IDistributedSubscriber<string, UdpMessage> _udpSubscriber;
        IUniTaskAsyncDisposable _udpSubscriberDisposable;

        /// <summary>依存注入</summary>
        [Inject] public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            NetworkInfoContainer networkInfoContainer,
            IDistributedPublisher<string, UdpMessage> udpPublisher,
            IDistributedSubscriber<string, UdpMessage> udpSubscriber)
            : base(
                "UdpBroadcastDemo ブートサービス",
                100,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"BootService.BootService()");
            _udpPublisher = udpPublisher;
            _udpSubscriber = udpSubscriber;
            _networkInfoContainer = networkInfoContainer;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"BootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
            // UDPメッセージ購読
            LogAddLine("UDPメッセージ購読処理登録");
            _udpSubscriberDisposable = await _udpSubscriber.SubscribeAsync(
                "BootService",
                async message => {
                    await UniTask.SwitchToMainThread();
                    LogAddLine($"UDPメッセージ受信: {message.Text}", ServiceLogColor);
                },
                cancellationToken: ct
            );
            LogAddLine("UDPメッセージ購読処理登録完了の筈");
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogAddLine($"BootService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機

            // 1秒おきにメッセージ送信（ctがキャンセルされるまで）
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
                await _udpPublisher.PublishAsync("BootService", UdpMessage.Create($"Hello I'm {_networkInfoContainer.NetworkInfo.LocalIPAddress} !!"), ct);
                LogAddLine("UDPメッセージ送信");
            }
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogAddLine($"BootService.Dispose()", ServiceLogColor);
            _udpSubscriberDisposable?.DisposeAsync().Forget();
        }
    }
}
