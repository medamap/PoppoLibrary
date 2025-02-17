using System.Threading;
using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.Boot.Application;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Domain;
using PoppoKoubou.CommonLibrary.AggregateService.Application;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using PoppoKoubou.CommonLibrary.Network.Application;
using PoppoKoubou.CommonLibrary.Network.Domain;
using PoppoKoubou.CommonLibrary.Network.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope
{
    public class UdpBroadcastLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override async void Configure(IContainerBuilder builder)
        {
            Debug.Log($"UdpBroadcastLifetimeScope.Configure()");
            
            // ネットワークのブロードキャストアドレスを自動検知する
            var networkInfoProvider = new NetworkInfoProvider();
            var networkInfo = networkInfoProvider.GetNetworkInfo(CancellationToken.None);
            Debug.Log($"Local IP: {networkInfo.LocalIPAddress}");
            Debug.Log($"Gateway: {networkInfo.DefaultGateway}");
            Debug.Log($"Subnet Mask: {networkInfo.SubnetMask}");
            Debug.Log($"Network Address: {networkInfo.NetworkAddress}");
            Debug.Log($"Broadcast Address: {networkInfo.BroadcastAddress}");

            // MessagePipeでメッセージを送受信するためのサービスを登録
            var options = builder.RegisterMessagePipe();
            var udpOptions = builder.ToMessagePipeBuilder().AddUdpInterprocess(networkInfo.LocalIPAddress, 37893, _ => { });
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, UdpMessage>(udpOptions);

            //// Message //////////////////////////////////////////////////
            
            // Log をメッセージ登録
            builder.RegisterMessageBroker<LogMessage>(options);
            // サービス集約ハブステータスをメッセージ登録
            builder.RegisterMessageBroker<CentralHubStatus>(options);
            // サービスノードステータスをメッセージ登録
            builder.RegisterMessageBroker<ServiceNodeStatus>(options);

            //// Log ////////////////////////////////////////////////////
            
            // ILogProvider を UnityLogProvider として登録
            builder.Register<ILogProvider, UnityLogProvider>(Lifetime.Singleton);
            // ILogOperator を TextMeshProUguiLogOperator として登録
            builder.Register<ILogOperator, TextMeshProUguiLogOperator>(Lifetime.Transient);
            // ILogFormatter を UnityLogFormatter として登録
            builder.Register<ILogFormatter, UnityLogFormatter>(Lifetime.Singleton);

            //// Network //////////////////////////////////////////////////

            // INetworkInfoProvider を NetworkInfoProvider として登録
            builder.Register<INetworkInfoProvider, NetworkInfoProvider>(Lifetime.Singleton);
            // ネットワーク情報コンテナを登録
            builder.Register<NetworkInfoContainer>(Lifetime.Singleton);
            
            //// Boot ///////////////////////////////////////////////////

            //// Entry Point ////////////////////////////////////////////
            
            // サービス集約ハブをエントリポイントに登録
            builder.RegisterEntryPoint<CentralHub>();
            // ログサービスをエントリポイントに登録
            builder.RegisterEntryPoint<LogService>();
            // ブートサービスエントリポイント登録
            builder.RegisterEntryPoint<BootService>();
            // ネットワークサービスエントリポイント登録
            builder.RegisterEntryPoint<NetworkService>();
        }
    }
}
