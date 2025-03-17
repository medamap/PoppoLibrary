using MessagePipe;
using PoppoKoubou_Demo.LogDemo.LogDemoBoot.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.VContainerCustom.Presentation;
using VContainer;

namespace PoppoKoubou_Demo.LogDemo.LifetimeScope
{
    public class LogDemoLifetimeScope : LifetimeScopeBehaviour
    {
        public MessagePipeOptions Options => MessagePipeOptions;

        /// <summary>ライフタイムスコープ初期化</summary>
        protected override void OnInitialize()
        {
            AvailableServices = Services.Log | Services.UI;
            GlobalLogSettings.LogLevel = LogLevel.All;
        }

        /// <summary>メッセージ登録</summary>
        protected override void OnRegisterMessage(IContainerBuilder builder, MessagePipeOptions options)
        {
            this.AddLogDemoBootMessage(builder, options);
        }

        /// <summary>コンポーネント登録</summary>
        protected override void OnRegisterComponent(IContainerBuilder builder)
        {
            this.AddLogDemoBootComponent(builder);
        }

        /// <summary>エントリーポイント登録</summary>
        protected override void OnRegisterEntryPoint(IContainerBuilder builder)
        {
            this.AddLogDemoBootEntryPoint(builder);
        }

        /// <summary>リソース開放</summary>
        protected override void OnDispose() { }
    }
}
