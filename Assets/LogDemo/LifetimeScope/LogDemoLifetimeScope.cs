using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Application;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LogDemo.LifetimeScope
{
    public class LogDemoLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"PowerCrashLifetimeScope.Configure()");
            
            // MessagePipeでメッセージを送受信するためのサービスを登録
            var options = builder.RegisterMessagePipe();

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

            //// Boot ///////////////////////////////////////////////////
            
            // ブートサービス登録
            builder.Register<Boot.Application.BootService>(Lifetime.Singleton);

            //// Entry Point ////////////////////////////////////////////
            
            // サービス集約ハブをエントリポイントに登録
            builder.RegisterEntryPoint<CentralHub>();
            // ログサービスをエントリポイントに登録
            builder.RegisterEntryPoint<LogService>();
            // ブートサービスエントリポイント登録
            builder.RegisterEntryPoint<Boot.Application.BootService>();

        }
    }
}
