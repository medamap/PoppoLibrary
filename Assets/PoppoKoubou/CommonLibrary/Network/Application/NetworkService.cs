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
        private readonly INetworkInfoContainer _networkInfoContainer;

        /// <summary>依存注入</summary>
        [Inject] public NetworkService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoProvider networkInfoProvider,
            INetworkInfoContainer networkInfoContainer)
            : base(
                "Network Service",
                90,
                logPublisher,
                centralHubStatusSubscriber,
                serviceNodeStatusPublisher)
        {
            LogPublisher.Debug($"NetworkService.NetworkService()");
            _networkInfoProvider = networkInfoProvider;
            _networkInfoContainer = networkInfoContainer;
        }

        /// <summary>サービス初期化</summary>
        protected override async UniTask StartInitialize(CancellationToken ct)
        {
            LogPublisher.Debug($"NetworkService.StartInitialize()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
            // ネットワーク情報取得
            LogPublisher.Debug("ネットワーク情報取得", ServiceLogColor);
            var networkInfo = await _networkInfoProvider.GetNetworkInfoAsync(ct);
            if (networkInfo is { IsError: false })
            {
                LogPublisher.Debug("ネットワーク情報", ServiceLogColor);
                LogPublisher.Debug("Local IP : " + networkInfo.LocalIPAddress, ServiceLogColor);
                LogPublisher.Debug("Gateway : " + networkInfo.DefaultGateway, ServiceLogColor);
                LogPublisher.Debug("Subnet Mask : " + networkInfo.SubnetMask, ServiceLogColor);
                LogPublisher.Debug("Network Address : " + networkInfo.NetworkAddress, ServiceLogColor);
                LogPublisher.Debug("Broadcast Address : " + networkInfo.BroadcastAddress, ServiceLogColor);
                _networkInfoContainer.SetNetworkInfo(networkInfo);
            }
            else
            {
                LogPublisher.Error("ネットワーク情報が取得できませんでした", ServiceLogColor);
                foreach (var log in networkInfo.LOG)
                {
                    LogPublisher.Error(log);
                }
            }
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.Debug($"NetworkService.StartService()", ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.Debug($"NetworkService.Dispose()", ServiceLogColor);
        }
    }
}