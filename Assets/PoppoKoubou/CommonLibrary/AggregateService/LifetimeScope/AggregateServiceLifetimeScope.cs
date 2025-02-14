using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.Application;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope
{
    public class AggregateServiceLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"AggregateServiceLifetimeScope.Configure()");

            // MessagePipeでメッセージを送受信するためのサービスを登録
            var options = builder.RegisterMessagePipe();

            //// Message //////////////////////////////////////////////////

            // サービス集約ハブステータスをメッセージ登録
            builder.RegisterMessageBroker<CentralHubStatus>(options);
            // サービスノードステータスをメッセージ登録
            builder.RegisterMessageBroker<ServiceNodeStatus>(options);

            //// Entry Point ////////////////////////////////////////////

            // サービス集約ハブをエントリポイントに登録
            builder.RegisterEntryPoint<CentralHub>();

        }
    }
}
