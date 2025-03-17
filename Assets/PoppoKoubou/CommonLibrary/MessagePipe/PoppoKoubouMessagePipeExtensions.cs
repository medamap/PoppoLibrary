using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePipe;
using MessagePipe.Interprocess;
using MessagePipe.Interprocess.Workers;
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
                new TcpMessageFormatter(),
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

        /// <summary>ぽっぽライブラリのUDPカスタムメッセージブローカを登録</summary>
        public static void RegisterPoppoKoubouInterprocessUdpMessageBroker(this IContainerBuilder builder, MessagePipeInterprocessOptions options)
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
        /// <summary>ぽっぽライブラリのTCPカスタムメッセージブローカを登録</summary>
        public static void RegisterPoppoKoubouInterprocessTcpMessageBroker(this IContainerBuilder builder, MessagePipeInterprocessOptions options)
        {
            // ここからおまじない開始 ----------
            // TcpWorkerを明示的に登録
            builder.Register<TcpWorker>(Lifetime.Singleton).WithParameter(options);
            // すべての基底クラスを明示的に登録
            builder.RegisterInstance(options);
            if (options is MessagePipeInterprocessTcpOptions tcpOptions)
            {
                builder.RegisterInstance(tcpOptions);
        
                if (options is MessagePipeInterprocessTcpExtendedOptions extendedOptions)
                {
                    //builder.RegisterInstance(extendedOptions);
                }
            }
            // おまじない終わり（これは必ず直す） ----------
            builder.ToMessagePipeBuilder().RegisterTcpInterprocessMessageBroker<string, TcpMessage>(options);
        }
    }
}
