using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.Log.LifetimeScope
{
    public class LogLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"LogLifetimeScope.Configure()");

            // MessagePipeでメッセージを送受信するためのサービスを登録
            var options = builder.RegisterMessagePipe();

            //// Message //////////////////////////////////////////////////

            // Log をメッセージ登録
            builder.RegisterMessageBroker<LogMessage>(options);

            //// Log ////////////////////////////////////////////////////

            // ILogProvider を UnityLogProvider として登録
            builder.Register<ILogProvider, UnityLogProvider>(Lifetime.Singleton);
            // ILogOperator を TextMeshProUguiLogOperator として登録
            builder.Register<ILogOperator, TextMeshProUguiLogOperator>(Lifetime.Transient);

            //// Entry Point ////////////////////////////////////////////

            // ログサービスをエントリポイントに登録
            builder.RegisterEntryPoint<LogService>();
        }
    }
}
