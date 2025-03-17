using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using MessagePipe.Interprocess;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.Network.Domain;
using R3;

// ReSharper disable ClassNeverInstantiated.Global

namespace PoppoKoubou_Demo.TcpNetworkDemo.TcpCommunication.Application
{
    /// <summary>TCP通信サービス</summary>
    public class TcpCommunicationService : ServiceNode
    {
        private readonly INetworkInfoContainer _networkInfoContainer;
        /// <summary>TCPメッセージ発行</summary>
        private readonly TcpDistributedPublisher<string, TcpMessage> _tcpMessagePublisher;
        /// <summary>TCPメッセージ購読</summary>
        private readonly IDistributedSubscriber<string, TcpMessage> _tcpMessageSubscriber;
        private readonly CompositeDisposable _disposables = new ();
        IUniTaskAsyncDisposable _tcpSubscriberDisposable;

        /// <summary>依存注入</summary>
        public TcpCommunicationService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoContainer networkInfoContainer,
            IDistributedPublisher<string, TcpMessage> tcpMessagePublisher,
            IDistributedSubscriber<string, TcpMessage> tcpMessageSubscriber
        ) : base(
            "(Child) TCP Service",
            120,
            logPublisher,
            centralHubStatusSubscriber,
            serviceNodeStatusPublisher
        ) {
            logPublisher.Debug("TcpCommunicationService.BootService()");
            _networkInfoContainer = networkInfoContainer;
            _tcpMessagePublisher = tcpMessagePublisher as TcpDistributedPublisher<string, TcpMessage>;
            _tcpMessageSubscriber = tcpMessageSubscriber;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.Debug("TcpCommunicationService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(1, cancellationToken: ct);
            // UDPメッセージ購読
            LogPublisher.Debug("TCPメッセージ購読処理登録","#40a0ff");
            _tcpSubscriberDisposable = await _tcpMessageSubscriber.SubscribeAsync(
                "BootService",
                async message => {
                    await UniTask.SwitchToMainThread();
                    LogPublisher.Debug($"TCPメッセージ受信: {message.Text}", ServiceLogColor);
                },
                cancellationToken: ct
            );
            LogPublisher.Debug("TCPメッセージ購読処理登録完了の筈", "#40a0ff");
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Debug("TcpCommunicationService.StartService()", ServiceLogColor);
            // 1秒おきにメッセージ送信（ctがキャンセルされるまで）
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
                LogPublisher.Debug("TCPメッセージ送信", ServiceLogColor);
                await _tcpMessagePublisher.CreatePublisher()
                    .WithRetry(3)
                    .WithTimeout(TimeSpan.FromSeconds(2))
                    .WaitForCompletion()
                    .WithErrorCallback(ex => LogPublisher.Error(ex.Message))
                    .PublishAsync(
                        "BootService",
                        TcpMessage.Create(
                            "127.0.0.1", // from address
                            "127.0.0.1", // to address
                            37564,
                            $"Hello I'm {_networkInfoContainer.NetworkInfo.LocalIPAddress} !!"),
                        cancellationToken: ct);
            }
        }

        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.Debug("TcpCommunicationService.Dispose()", ServiceLogColor);
            _disposables?.Dispose();
            _tcpSubscriberDisposable?.DisposeAsync();
        }
    }
}