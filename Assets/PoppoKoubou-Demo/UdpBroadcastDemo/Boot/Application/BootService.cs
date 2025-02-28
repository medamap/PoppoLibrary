using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;
using VContainer.Unity;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Domain;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.Boot.Application
{
    /// <summary>ブートサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BootService : ServiceNode
    {
        /// <summary>ログAPI</summary>
        private readonly LogApi _logApi;
        /// <summary>ネットワークサービスコンテナ</summary>
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>UDPブロードキャストライフタイムスコープ</summary>
        private readonly UdpBroadcastLifetimeScope _lifetimeScope;
        
        /// <summary>依存注入</summary>
        [Inject] public BootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            LogApi logApi,
            INetworkInfoContainer networkInfoContainer,
            UdpBroadcastLifetimeScope lifetimeScope)
            : base(
                "UdpBroadcastDemo ブートサービス",
                110,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"UdpBroadcastDemo.Boot.Application.BootService.BootService()");
            _logApi = logApi;
            _networkInfoContainer = networkInfoContainer;
            _lifetimeScope = lifetimeScope;
            _logApi.UpdateLogLevel(LogLevel.All);
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"UdpBroadcastDemo.Boot.Application.BootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機

            var child = _lifetimeScope.CreateChild(builder =>
            {
                //// UDP Interprocess //////////////////////////////////////////////////

                var resolver = builder.CreatePoppoKoubouCompositeResolver();
                var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
                
                // UDP メッセージ送受信用のオプションを設定（ここで MessagePackSerializerOptions を上書き）
                var udpOptions = builder.ToMessagePipeBuilder()
                    .AddUdpInterprocessWithSubnet(
                        _networkInfoContainer.NetworkInfo.BroadcastAddress,
                        37893,
                        _networkInfoContainer.NetworkInfo.SubnetMask,
                        _networkInfoContainer.NetworkInfo.NetworkAddress,
                        opt => {
                        opt.MessagePackSerializerOptions = options;
                    });

                // UDP メッセージブローカー登録
                builder.RegisterPoppoKoubouInterprocessMessageBroker(udpOptions);
                
                //// UDP Communication //////////////////////////////////////////////////

                // UDP通信サービスエントリポイント登録
                builder.RegisterEntryPoint<PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Application.UdpCommunicationService>();

            });
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.AddLine($"UdpBroadcastDemo.Boot.Application.BootService.StartService()", LogLevel.Debug, ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.AddLine($"UdpBroadcastDemo.Boot.Application.BootService.Dispose()", LogLevel.Debug, ServiceLogColor);
        }
    }
}
