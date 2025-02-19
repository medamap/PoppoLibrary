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

namespace PoppoKoubou.CommonLibrary.Network.Application
{
    /// <summary>ネットワークサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NetworkService : ServiceNode
    {
        /// <summary>ネットワーク情報プロバイダ</summary>
        private readonly INetworkInfoProvider _networkInfoProvider;
        /// <summary>ネットワーク情報コンテナ</summary>
        public INetworkInfoContainer NetworkInfoContainer => _networkInfoContainer;
        private INetworkInfoContainer _networkInfoContainer;

        /// <summary>依存注入</summary>
        [Inject] public NetworkService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoProvider networkInfoProvider,
            INetworkInfoContainer networkInfoContainer)
            : base(
                "ネットワークサービス",
                100,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            Debug.Log($"NetworkService.NetworkService()");
            _networkInfoProvider = networkInfoProvider;
            _networkInfoContainer = networkInfoContainer;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogAddLine($"NetworkService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
            // ネットワーク情報取得
            LogAddLine("ネットワーク情報取得");
            var networkInfo = await _networkInfoProvider.GetNetworkInfoAsync(ct);
            if (networkInfo is { IsError: false })
            {
                LogAddLine("ネットワーク情報");
                LogAddLine("Local IP : " + networkInfo.LocalIPAddress);
                LogAddLine("Gateway : " + networkInfo.DefaultGateway);
                LogAddLine("Subnet Mask : " + networkInfo.SubnetMask);
                LogAddLine("Network Address : " + networkInfo.NetworkAddress);
                LogAddLine("Broadcast Address : " + networkInfo.BroadcastAddress);
                _networkInfoContainer.SetNetworkInfo(networkInfo);
            }
            else
            {
                LogAddLine("ネットワーク情報が取得できませんでした");
                foreach (var log in networkInfo.LOG)
                {
                    ErrorAddLine(log);
                }
            }
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogAddLine($"NetworkService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogAddLine($"NetworkService.Dispose()", ServiceLogColor);
        }
    }
}