using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using UnityEngine;

namespace PoppoKoubou.CommonLibrary.Network.Application
{
    /// <summary>ネットワークサービス</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NetworkService : ServiceNode
    {
        /// <summary>ネットワーク情報プロバイダ</summary>
        private readonly INetworkInfoProvider _networkInfoProvider;
        /// <summary>ネットワークサービスコンテナ</summary>
        public NetworkInfoContainer NetworkInfoContainer => _networkInfoContainer;
        private NetworkInfoContainer _networkInfoContainer;

        /// <summary>サービスコンストラクタ</summary>
        public NetworkService(
            IPublisher<LogMessage> logPublisher,
            ISubscriber<CentralHubStatus> centralHubStatusSubscriber,
            IPublisher<ServiceNodeStatus> serviceNodeStatusPublisher,
            INetworkInfoProvider networkInfoProvider,
            NetworkInfoContainer networkInfoContainer)
            : base("ネットワークサービス", 100,
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
            var _networkInfo = await _networkInfoProvider.GetNetworkInfoAsync(ct);
            if (_networkInfo != null)
            {
                LogAddLine("ネットワーク情報");
                LogAddLine("Local IP : " + _networkInfo.LocalIPAddress);
                LogAddLine("Gateway : " + _networkInfo.DefaultGateway);
                LogAddLine("Subnet Mask : " + _networkInfo.SubnetMask);
                LogAddLine("Network Address : " + _networkInfo.NetworkAddress);
                LogAddLine("Broadcast Address : " + _networkInfo.BroadcastAddress);
                _networkInfoContainer.SetNetworkInfo(_networkInfo);
            }
            else
            {
                LogAddLine("ネットワーク情報が取得できませんでした");
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