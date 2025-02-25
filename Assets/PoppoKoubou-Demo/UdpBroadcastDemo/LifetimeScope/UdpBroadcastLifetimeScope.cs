using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.LifetimeScope;
using PoppoKoubou.CommonLibrary.Network.LifetimeScope;
using PoppoKoubou.CommonLibrary.UI.LifetimeScope;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope
{
    public class UdpBroadcastLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"UdpBroadcastLifetimeScope.Configure()");
            var options = builder.RegisterMessagePipe();

            // メッセージ登録
            this.AddAggregateServiceMessage(builder, options);
            this.AddLogMessage(builder, options);
            this.AddNetworkMessage(builder, options);
            this.AddUIMessage(builder, options);

            // コンポーネント登録
            this.AddAggregateServiceComponent(builder);
            this.AddLogComponent(builder);
            this.AddNetworkComponent(builder);
            this.AddUIComponent(builder);
            
            // エントリポイント登録
            this.AddAggregateServiceEntryPoint(builder);
            this.AddLogEntryPoint(builder);
            this.AddNetworkEntryPoint(builder);
            this.AddUIEntryPoint(builder);
            builder.RegisterEntryPoint<UdpBroadcastDemo.Boot.Application.BootService>();
        }
    }
}
