using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Application
{
    /// <summary>UDP通信サービス</summary>
    public class UdpCommunicationService : ServiceNode
    {
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>UDPメッセージパブリッシャー</summary>
        private readonly IDistributedPublisher<string, UdpMessage> _udpPublisher;
        /// <summary>UDPメッセージパブリッシャー</summary>
        private readonly IDistributedSubscriber<string, UdpMessage> _udpSubscriber;
        IUniTaskAsyncDisposable _udpSubscriberDisposable;

        /// <summary>依存注入</summary>
        [Inject] public UdpCommunicationService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoContainer networkInfoContainer,
            IDistributedPublisher<string, UdpMessage> udpPublisher,
            IDistributedSubscriber<string, UdpMessage> udpSubscriber)
            : base(
                "(Child) UDP通信サービス",
                120,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"UdpCommunicationService.UdpCommunicationService()");
            _networkInfoContainer = networkInfoContainer;
            _udpPublisher = udpPublisher;
            _udpSubscriber = udpSubscriber;
        }

        /// <summary>サービス初期化 </summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"UdpCommunicationService.StartInitialize()", ServiceLogColor);
            // UDPメッセージ購読
            LogAddLine("UDPメッセージ購読処理登録", "#40a0ff");
            _udpSubscriberDisposable = await _udpSubscriber.SubscribeAsync(
                "BootService",
                async message => {
                    await UniTask.SwitchToMainThread();
                    LogAddLine($"UDPメッセージ受信: {message.Text}", ServiceLogColor);
                },
                cancellationToken: ct
            );
            LogAddLine("UDPメッセージ購読処理登録完了の筈", "#40a0ff");
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogAddLine($"UdpCommunicationService.StartService()", ServiceLogColor);
            // 1秒おきにメッセージ送信（ctがキャンセルされるまで）
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
                LogAddLine("UDPメッセージ送信");
                await _udpPublisher.PublishAsync("BootService", UdpMessage.Create($"Hello I'm {_networkInfoContainer.NetworkInfo.LocalIPAddress} !!"), ct);
            }
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogAddLine($"UdpCommunicationService.Dispose()", ServiceLogColor);
            _udpSubscriberDisposable?.DisposeAsync().Forget();
        }
    }
}