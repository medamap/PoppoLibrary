using MessagePipe;
using PoppoKoubou_Demo.TcpNetworkDemo.TcpNetworkDemoBoot.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.VContainerCustom.Presentation;
using VContainer;

// ReSharper disable MemberCanBePrivate.Global

namespace PoppoKoubou_Demo.TcpNetworkDemo.LifetimeScope
{
    public class TcpNetworkDemoLifetimeScope : LifetimeScopeBehaviour
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
            this.AddTcpNetworkDemoBootMessage(builder, Options);
        }

        /// <summary>コンポーネント登録</summary>
        protected override void OnRegisterComponent(IContainerBuilder builder)
        {
            this.AddTcpNetworkDemoBootComponent(builder);
        }

        /// <summary>エントリーポイント登録</summary>
        protected override void OnRegisterEntryPoint(IContainerBuilder builder)
        {
            this.AddTcpNetworkDemoBootEntryPoint(builder);
        }

        /// <summary>リソース開放</summary>
        protected override void OnDispose() { }
    }
}