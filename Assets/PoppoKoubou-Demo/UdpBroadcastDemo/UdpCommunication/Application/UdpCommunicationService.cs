using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;
using VContainer;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Application
{
    /// <summary>UDP通信サービス</summary>
    public class UdpCommunicationService : ServiceNode
    {
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>UDPメッセージ発行</summary>
        private readonly IDistributedPublisher<string, UdpMessage> _udpMessagePublisher;
        /// <summary>UDPメッセージ購読</summary>
        private readonly IDistributedSubscriber<string, UdpMessage> _udpMessageSubscriber;
        IUniTaskAsyncDisposable _udpSubscriberDisposable;

        /// <summary>依存注入</summary>
        [Inject] public UdpCommunicationService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoContainer networkInfoContainer,
            IDistributedPublisher<string, UdpMessage> udpMessagePublisher,
            IDistributedSubscriber<string, UdpMessage> udpMessageSubscriber)
            : base(
                "(Child) UDP通信サービス",
                120,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            logPublisher.Debug($"UdpCommunicationService.UdpCommunicationService()");
            _networkInfoContainer = networkInfoContainer;
            _udpMessagePublisher = udpMessagePublisher;
            _udpMessageSubscriber = udpMessageSubscriber;
        }

        /// <summary>サービス初期化 </summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.Debug($"UdpCommunicationService.StartInitialize()", ServiceLogColor);
            // UDPメッセージ購読
            LogPublisher.Debug("UDPメッセージ購読処理登録","#40a0ff");
            _udpSubscriberDisposable = await _udpMessageSubscriber.SubscribeAsync(
                "BootService",
                async message => {
                    await UniTask.SwitchToMainThread();
                    LogPublisher.Debug($"UDPメッセージ受信: {message.Text}", ServiceLogColor);
                },
                cancellationToken: ct
            );
            LogPublisher.Debug("UDPメッセージ購読処理登録完了の筈", "#40a0ff");
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Debug($"UdpCommunicationService.StartService()", ServiceLogColor);
            // 1秒おきにメッセージ送信（ctがキャンセルされるまで）
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
                LogPublisher.Debug("UDPメッセージ送信", ServiceLogColor);
                await _udpMessagePublisher.PublishAsync("BootService", UdpMessage.Create($"Hello I'm {_networkInfoContainer.NetworkInfo.LocalIPAddress} !!"), ct);
            }
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.Debug($"UdpCommunicationService.Dispose()", ServiceLogColor);
            _udpSubscriberDisposable?.DisposeAsync().Forget();
        }
    }
}