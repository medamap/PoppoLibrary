using MessagePipe;
using PoppoKoubou.CommonLibrary.UI.Application;
using PoppoKoubou.CommonLibrary.UI.Domain;
using PoppoKoubou.CommonLibrary.UI.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.UI.LifetimeScope
{
    public class UILifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"UILifetimeScope.Configure()");
            
            // MessagePipeでメッセージを送受信するためのサービスを登録
            var options = builder.RegisterMessagePipe();

            //// Message //////////////////////////////////////////////////

            // InteractUI をメッセージ登録
            builder.RegisterMessageBroker<InteractUI>(options);

            //// UI /////////////////////////////////////////////////

            // IInteractUIProvider　を InteractUIProvider として登録
            builder.Register<IInteractUIDispatch, InteractUIDispatch>(Lifetime.Singleton);

            //// EntryPoint /////////////////////////////////////////////////

            // UIサービスをエントリポイントに登録
            builder.RegisterEntryPoint<UIService>();
        }
    }
}
