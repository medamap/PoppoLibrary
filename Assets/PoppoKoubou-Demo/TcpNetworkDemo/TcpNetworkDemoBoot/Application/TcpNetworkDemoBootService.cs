using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePack;
using MessagePipe;
using MessagePipe.Interprocess;
using MessagePipe.Interprocess.Workers;
using PoppoKoubou_Demo.TcpNetworkDemo.LifetimeScope;
using PoppoKoubou_Demo.TcpNetworkDemo.TcpCommunication.LifetimeScope;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.MessagePipe;
using PoppoKoubou.CommonLibrary.Network.Domain;
using VContainer;

// ReSharper disable ClassNeverInstantiated.Global

namespace PoppoKoubou_Demo.TcpNetworkDemo.TcpNetworkDemoBoot.Application
{
    /// <summary>ブートサービス</summary>
    public class TcpNetworkDemoBootService : ServiceNode
    {
        /// <summary>ネットワークサービスコンテナ</summary>
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>TCPネットワークライフタイムスコープ</summary>
        private readonly TcpNetworkDemoLifetimeScope _demoLifetimeScope;
        
        /// <summary>依存注入</summary>
        public TcpNetworkDemoBootService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            LogApi logApi,
            INetworkInfoContainer networkInfoContainer,
            TcpNetworkDemoLifetimeScope demoLifetimeScope
        ) : base("Tcp Network Demo Boot Service",
            100,
            logPublisher,
            centralHubStatusSubscriber,
            serviceNodeStatusPublisher
        ) {
            logApi.UpdateLogLevel(LogLevel.All); // 出力対象のログレベル設定
            logPublisher.Debug("TcpNetworkDemoBootService.BootService()", ServiceLogColor);
            // 依存注入
            _networkInfoContainer = networkInfoContainer;
            _demoLifetimeScope = demoLifetimeScope;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.Debug("TcpNetworkDemoBootService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);

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

                // TCP通信の設定（コマンド送信用）
                var tcpOptions = new MessagePipeInterprocessTcpExtendedOptions(
                    "127.0.0.1",
                    37564)
                {
                    IgnoreConnectErrors = false,
                    IgnoreSendErrors = false,
                    MaxRetryCount = 3,
                    MessagePackSerializerOptions = serializerOptions
                };
                // すべての基底クラスを明示的に登録
                //builder.RegisterInstance<MessagePipeInterprocessOptions>(tcpOptions);
                //builder.RegisterInstance<MessagePipeInterprocessTcpOptions>(tcpOptions);
                //builder.RegisterInstance<MessagePipeInterprocessTcpExtendedOptions>(tcpOptions);
                // UDP メッセージブローカー登録
                builder.RegisterPoppoKoubouInterprocessUdpMessageBroker(udpOptions);
                // TCP メッセージブローカー登録
                builder.RegisterPoppoKoubouInterprocessTcpMessageBroker(tcpOptions);
                
                //// TCP Communication //////////////////////////////////////////////////
                // MessagePipeでメッセージを送受信するためのサービスを登録
                MessagePipeOptions options = TcpNetworkDemoLifetimeScope.Options;

                // Message ////////////////////////////////////////
                _demoLifetimeScope.AddTcpCommunicationMessage(builder, options);

                // Component ////////////////////////////////////////
                _demoLifetimeScope.AddTcpCommunicationComponent(builder);

                // EntryPoint ////////////////////////////////////////
                _demoLifetimeScope.AddTcpCommunicationEntryPoint(builder);


            });
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Debug("TcpNetworkDemoBootService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct);
        }

        /// <summary>リソース開放</summary>
        public override void Dispose()
        {
            LogPublisher.Debug("TcpNetworkDemoBootService.Dispose()", ServiceLogColor);
        }
    }
}