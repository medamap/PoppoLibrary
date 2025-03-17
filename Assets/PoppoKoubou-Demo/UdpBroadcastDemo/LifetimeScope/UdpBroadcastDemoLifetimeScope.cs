using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpBroadCastDemoBoot.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.VContainerCustom.Presentation;
using VContainer;

// ReSharper disable MemberCanBePrivate.Global

namespace PoppoKoubou_Demo.UdpBroadcastDemo.LifetimeScope
{
    public class UdpBroadcastDemoLifetimeScope : LifetimeScopeBehaviour
    {
        public static MessagePipeOptions Options;

        /// <summary>ライフタイムスコープ初期化</summary>
        protected override void OnInitialize()
        {
            AvailableServices = Services.Log | Services.UI | Services.Network;
            GlobalLogSettings.LogLevel = LogLevel.All;
            Options = MessagePipeOptions;
        }

        /// <summary>メッセージ登録</summary>
        protected override void OnRegisterMessage(IContainerBuilder builder, MessagePipeOptions options)
        {
            this.AddUdpBroadcastDemoBootMessage(builder, options);
        }

        /// <summary>コンポーネント登録</summary>
        protected override void OnRegisterComponent(IContainerBuilder builder)
        {
            this.AddUdpBroadcastDemoBootComponent(builder);
        }

        /// <summary>エントリーポイント登録</summary>
        protected override void OnRegisterEntryPoint(IContainerBuilder builder)
        {
            this.AddUdpBroadcastDemoBootEntryPoint(builder);
        }

        /// <summary>リソース開放</summary>
        protected override void OnDispose() { }
    }
}
