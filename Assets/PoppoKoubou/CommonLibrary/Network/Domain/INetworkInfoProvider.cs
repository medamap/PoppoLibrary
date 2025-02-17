using System.Threading;
using Cysharp.Threading.Tasks;

namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    public interface INetworkInfoProvider
    {
        public UniTask<NetworkInfo> GetNetworkInfoAsync(CancellationToken token);
    }
}