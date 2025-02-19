using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.Boot.Application
{
    /// <summary>ブートサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BootService : ServiceNode
    {
        /// <summary>ネットワークサービスコンテナ</summary>
        private readonly INetworkInfoContainer _networkInfoContainer;

        /// <summary>UDPブロードキャストライフタイムスコープ</summary>
        private readonly UdpBroadcastLifetimeScope _lifetimeScope;
        
        /// <summary>依存注入</summary>
        [Inject] public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            UdpBroadcastLifetimeScope lifetimeScope,
            INetworkInfoContainer networkInfoContainer)
            : base(
                "UdpBroadcastDemo ブートサービス",
                110,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"UdpBroadcastDemo.Boot.Application.BootService.BootService()");
            _lifetimeScope = lifetimeScope;
            _networkInfoContainer = networkInfoContainer;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"UdpBroadcastDemo.Boot.Application.BootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機

            var child = _lifetimeScope.CreateChild(builder =>
            {
                //// UDP Interprocess //////////////////////////////////////////////////

                // UnityWebRequest でgoogleにリクエストする
                var request = UnityWebRequest.Get ("http://www.google.com");

                // UDP メッセージ送受信用を設定
                var udpOptions = builder.ToMessagePipeBuilder()
                    .AddUdpInterprocess(_networkInfoContainer.NetworkInfo.BroadcastAddress, 37893, _ => { });
                builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, UdpMessage>(udpOptions);
                
                //// UDP Communication //////////////////////////////////////////////////

                // UDP通信サービスエントリポイント登録
                builder.RegisterEntryPoint<PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Application.UdpCommunicationService>();

            });
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogAddLine($"UdpBroadcastDemo.Boot.Application.BootService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogAddLine($"UdpBroadcastDemo.Boot.Application.BootService.Dispose()", ServiceLogColor);
        }
    }
}
