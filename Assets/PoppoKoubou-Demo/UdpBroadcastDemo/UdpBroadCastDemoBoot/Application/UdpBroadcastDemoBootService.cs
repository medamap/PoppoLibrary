using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using MessagePipe;
using MessagePipe.Interprocess;
using PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.LifetimeScope;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.MessagePipe;
using PoppoKoubou.CommonLibrary.Network.Domain;
using VContainer;

// ReSharper disable SuggestVarOrType_SimpleTypes

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpBroadCastDemoBoot.Application
{
    /// <summary>ブートサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UdpBroadcastDemoBootService : ServiceNode
    {
        /// <summary>ネットワークサービスコンテナ</summary>
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>UDPブロードキャストライフタイムスコープ</summary>
        private readonly UdpBroadcastDemoLifetimeScope _demoLifetimeScope;
        
        /// <summary>依存注入</summary>
        [Inject] public UdpBroadcastDemoBootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            LogApi logApi,
            INetworkInfoContainer networkInfoContainer,
            UdpBroadcastDemoLifetimeScope demoLifetimeScope
        ) : base(
                "UdpBroadcastDemo ブートサービス",
                110,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            logApi.UpdateLogLevel(LogLevel.All);
            logPublisher.Debug($"UdpBroadcastDemoBootService.BootService()");
            _networkInfoContainer = networkInfoContainer;
            _demoLifetimeScope = demoLifetimeScope;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.Debug($"UdpBroadcastDemoBootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機

            var child = _demoLifetimeScope.CreateChild(builder =>
            {
                //// UDP Interprocess //////////////////////////////////////////////////

                IFormatterResolver resolver = builder.CreatePoppoKoubouCompositeResolver();
                MessagePackSerializerOptions serializerOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
                
                // UDP メッセージ送受信用のオプションを設定（ここで MessagePackSerializerOptions を上書き）
                var udpOptions = builder.ToMessagePipeBuilder()
                    .AddUdpInterprocessWithSubnet(
                        _networkInfoContainer.NetworkInfo.BroadcastAddress,
                        37893,
                        _networkInfoContainer.NetworkInfo.SubnetMask,
                        _networkInfoContainer.NetworkInfo.NetworkAddress,
                        true,
                        true,
                        opt => {
                        opt.MessagePackSerializerOptions = serializerOptions;
                    });

                // UDP メッセージブローカー登録
                builder.RegisterPoppoKoubouInterprocessUdpMessageBroker(udpOptions);
                
                //// UDP Communication //////////////////////////////////////////////////
                // MessagePipeでメッセージを送受信するためのサービスを登録
                MessagePipeOptions options = UdpBroadcastDemoLifetimeScope.Options;

                // Message ////////////////////////////////////////
                _demoLifetimeScope.AddUdpCommunicationMessage(builder, options);

                // Component ////////////////////////////////////////
                _demoLifetimeScope.AddUdpCommunicationComponent(builder);

                // EntryPoint ////////////////////////////////////////
                _demoLifetimeScope.AddUdpCommunicationEntryPoint(builder);
            });
        }
        
        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Debug($"UdpBroadcastDemoBootService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.Debug($"UdpBroadcastDemoBootService.Dispose()", ServiceLogColor);
        }
    }
}
