using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.LifetimeScope;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.LogDemo.LifetimeScope
{
    public class LogDemoLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"LogDemoLifetimeScope.Configure()");
            var options = builder.RegisterMessagePipe();

            // メッセージ登録
            this.AddAggregateServiceMessage(builder, options);
            this.AddLogMessage(builder, options);

            // コンポーネント登録
            this.AddAggregateServiceComponent(builder);
            this.AddLogComponent(builder);

            // エントリポイント登録
            this.AddAggregateServiceEntryPoint(builder);
            this.AddLogEntryPoint(builder);
            builder.RegisterEntryPoint<LogDemo.Boot.Application.BootService>();
        }
    }
}
