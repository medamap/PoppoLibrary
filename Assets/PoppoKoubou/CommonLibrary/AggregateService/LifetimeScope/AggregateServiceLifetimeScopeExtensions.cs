using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Application;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope
{
    public static class AggregateServiceLifetimeScopeExtensions
    {
        public static void AddAggregateServiceMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options)
        {
            // サービス集約ハブステータスをメッセージ登録
            builder.RegisterMessageBroker<CentralHubStatus>(options);
            // サービスノードステータスをメッセージ登録
            builder.RegisterMessageBroker<ServiceNodeStatus>(options);
        }
        public static void AddAggregateServiceComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
        }
        public static void AddAggregateServiceEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CentralHub>();
        }
    }
}