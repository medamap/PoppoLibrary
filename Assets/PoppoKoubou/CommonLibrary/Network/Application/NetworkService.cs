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
            LogPublisher.AddLine($"NetworkService.StartInitialize()", LogLevel.Debug, ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
            // ネットワーク情報取得
            LogPublisher.AddLine("ネットワーク情報取得", LogLevel.Debug, ServiceLogColor);
            var networkInfo = await _networkInfoProvider.GetNetworkInfoAsync(ct);
            if (networkInfo is { IsError: false })
            {
                LogPublisher.AddLine("ネットワーク情報", LogLevel.Debug, ServiceLogColor);
                LogPublisher.AddLine("Local IP : " + networkInfo.LocalIPAddress, LogLevel.Debug, ServiceLogColor);
                LogPublisher.AddLine("Gateway : " + networkInfo.DefaultGateway, LogLevel.Debug, ServiceLogColor);
                LogPublisher.AddLine("Subnet Mask : " + networkInfo.SubnetMask, LogLevel.Debug, ServiceLogColor);
                LogPublisher.AddLine("Network Address : " + networkInfo.NetworkAddress, LogLevel.Debug, ServiceLogColor);
                LogPublisher.AddLine("Broadcast Address : " + networkInfo.BroadcastAddress, LogLevel.Debug, ServiceLogColor);
                _networkInfoContainer.SetNetworkInfo(networkInfo);
            }
            else
            {
                LogPublisher.AddLine("ネットワーク情報が取得できませんでした", LogLevel.Error, ServiceLogColor);
                foreach (var log in networkInfo.LOG)
                {
                    ErrorAddLine(log);
                }
            }
        }

        /// <summary>サービス開始</summary>
        protected override async UniTask StartService(CancellationToken ct)
        {
            LogPublisher.AddLine($"NetworkService.StartService()", LogLevel.Debug, ServiceLogColor);
            await UniTask.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: ct); // 1ミリ秒待機
        }
        
        /// <summary>リソース解放</summary>
        public override void Dispose()
        {
            LogPublisher.AddLine($"NetworkService.Dispose()", LogLevel.Debug, ServiceLogColor);
        }
    }
}