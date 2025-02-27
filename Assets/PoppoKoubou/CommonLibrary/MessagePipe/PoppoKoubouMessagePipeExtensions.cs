using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
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
        public static IFormatterResolver CreatePoppoKoubouCompositeResolver(this IContainerBuilder builder, params IMessagePackFormatter[] additionalFormatters)
        {
            // 既定のカスタムフォーマッター群
            IMessagePackFormatter[] fixedFormatters =
            {
                new CentralHubStatusFormatter(),
                new ServiceNodeInfoFormatter(),
                new LogMessageFormatter(),
                new NetworkInfoFormatter(),
                new UdpMessageFormatter(),
                new InteractUIFormatter(),
                new UpdateUIFormatter(),
            };

            // 固定フォーマッターと追加フォーマッターを結合
            IMessagePackFormatter[] combinedFormatters;
            if (additionalFormatters is { Length: > 0 })
            {
                combinedFormatters = new IMessagePackFormatter[fixedFormatters.Length + additionalFormatters.Length];
                fixedFormatters.CopyTo(combinedFormatters, 0);
                additionalFormatters.CopyTo(combinedFormatters, fixedFormatters.Length);
            }
            else
            {
                combinedFormatters = fixedFormatters;
            }

            return CompositeResolver.Create(
                combinedFormatters,
                new IFormatterResolver[] { ContractlessStandardResolver.Instance }
            );
        }
    }
}
