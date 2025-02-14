using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービスノードインターフェイス</summary>
    public interface IServiceNode : IAsyncStartable
    {
        /// <summary>サービスノードス情報</summary>
        ServiceNodeInfo ServiceNodeInfo { get; }
    }
}
