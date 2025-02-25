using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Application;
using PoppoKoubou.CommonLibrary.UI.Domain;
using PoppoKoubou.CommonLibrary.UI.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.UI.LifetimeScope
{
    public static class UILifetimeScopeExtensions
    {
        public static void AddUIMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<InteractUI>(options); // InteractUI をメッセージ登録
            builder.RegisterMessageBroker<UpdateUI>(options); // UpdateUI をメッセージ登録
        }
        public static void AddUIComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            // IInteractUIProvider　を InteractUIProvider として登録
            builder.Register<IInteractUIDispatch, InteractUIDispatch>(Lifetime.Singleton);
        }
        public static void AddUIEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<UIService>();
        }
    }
}