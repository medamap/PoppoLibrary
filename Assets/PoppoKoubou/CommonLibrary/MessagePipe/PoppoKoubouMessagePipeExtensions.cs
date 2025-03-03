using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePipe;
using MessagePipe.Interprocess;
using PoppoKoubou.CommonLibrary.AggregateService.Domain;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Network.Domain;
using PoppoKoubou.CommonLibrary.UI.Domain;
using VContainer;

namespace PoppoKoubou.CommonLibrary.MessagePipe
{
    public static class PoppoKoubouMessagePipeExtensions
    {
        /// <summary>
        /// 固定のカスタムフォーマッターに加え、追加のカスタムフォーマッターを追記した CompositeResolver を生成します。
        /// </summary>
        /// <param name="builder">IContainerBuilder インスタンス</param>
        /// <param name="additionalFormatters">追加のカスタムフォーマッター</param>
        /// <returns>CompositeResolver</returns>
        public static IFormatterResolver CreatePoppoKoubouCompositeResolver(this IContainerBuilder builder, IMessagePackFormatter[] additionalFormatters = null)
        {
            // 既定のカスタムフォーマッター群
            var fixedFormatters = new List<IMessagePackFormatter>()
            {
                new CentralHubStatusFormatter(),
                new ServiceNodeInfoFormatter(),
                new LogMessageFormatter(),
                new NetworkInfoFormatter(),
                new UdpMessageFormatter(),
                new InteractUIFormatter(),
                new ClickUIFormatter(),
                new UpdateUIFormatter(),
            };
            // 固定フォーマッターと追加フォーマッターを結合
            if (additionalFormatters is { Length: > 0 })
            {
                fixedFormatters.AddRange(additionalFormatters);
            }

            return CompositeResolver.Create(
                fixedFormatters.ToArray(),
                new IFormatterResolver[] { ContractlessStandardResolver.Instance }
            );
        }

        /// <summary>ぽっぽライブラリのカスタムメッセージブローカを登録</summary>
        public static void RegisterPoppoKoubouInterprocessMessageBroker(this IContainerBuilder builder, MessagePipeInterprocessOptions options)
        {
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, CentralHubStatus>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, ServiceNodeInfo>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, LogMessage>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, NetworkInfo>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, UdpMessage>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, InteractUI>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, ClickUI>(options);
            builder.ToMessagePipeBuilder().RegisterUpdInterprocessMessageBroker<string, UpdateUI>(options);
        }
    }
}
