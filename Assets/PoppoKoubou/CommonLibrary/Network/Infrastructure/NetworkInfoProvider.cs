using PoppoKoubou.CommonLibrary.Network.Domain;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PoppoKoubou.CommonLibrary.Network.Infrastructure
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NetworkInfoProvider : INetworkInfoProvider
    {
        public async UniTask<NetworkInfo> GetNetworkInfoAsync(CancellationToken token)
        {
            return await UniTask.RunOnThreadPool(() => GetNetworkInfo(token), cancellationToken: token);
        }

        public NetworkInfo GetNetworkInfo(CancellationToken token)
        {
            // キャンセルチェック
            token.ThrowIfCancellationRequested();

            // 稼働中かつループバック以外のネットワークインターフェースを取得
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            token.ThrowIfCancellationRequested();

            // 各インターフェースにスコアを付与して、最もスコアの高い候補を選択
            var candidate = interfaces
                .Select(ni => new { Interface = ni, Score = ComputeInterfaceScore(ni) })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            token.ThrowIfCancellationRequested();

            if (candidate == null)
                return null;

            var selectedInterface = candidate.Interface;
            var ipProps = selectedInterface.GetIPProperties();

            // IPv4 のデフォルトゲートウェイ（ルータIP）を取得
            var gatewayAddress = ipProps.GatewayAddresses
                .FirstOrDefault(g => g.Address.AddressFamily == AddressFamily.InterNetwork)?.Address;
            if (gatewayAddress == null)
                return null;

            token.ThrowIfCancellationRequested();

            // IPv4 の Unicast アドレス情報（IPv4Mask が取得できるもの）を取得
            var unicastInfo = ipProps.UnicastAddresses
                .FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork && u.IPv4Mask != null);
            if (unicastInfo == null)
                return null;

            token.ThrowIfCancellationRequested();

            var localIP = unicastInfo.Address;
            var subnetMask = unicastInfo.IPv4Mask;

            // ネットワークアドレス = ローカルIP と サブネットマスクの AND
            var ipBytes = localIP.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            if (ipBytes.Length != maskBytes.Length)
                return null;

            var networkBytes = new byte[ipBytes.Length];
            for (var i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }
            var networkAddress = new IPAddress(networkBytes);

            token.ThrowIfCancellationRequested();

            // ブロードキャストアドレス = ネットワークアドレス OR (NOT サブネットマスク)
            var broadcastBytes = new byte[ipBytes.Length];
            for (var i = 0; i < ipBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(networkBytes[i] | (~maskBytes[i]));
            }
            var broadcastAddress = new IPAddress(broadcastBytes);

            token.ThrowIfCancellationRequested();

            return new NetworkInfo(
                localIP.ToString(),
                gatewayAddress.ToString(),
                subnetMask.ToString(),
                networkAddress.ToString(),
                broadcastAddress.ToString());
        }

        /// <summary>
        /// 各ネットワークインターフェースにスコアを付与する。
        /// - 有線（Ethernet, GigabitEthernet）に高いスコア
        /// - Wireless80211 は中程度のスコア
        /// - 速度や APIPA (169.254.x.x) の場合に補正
        /// </summary>
        private double ComputeInterfaceScore(NetworkInterface ni)
        {
            double score = 0;

            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                ni.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet)
            {
                score += 10000;
            }
            else if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                score += 5000;
            }
            else
            {
                score += 1000;
            }

            // Speed を Mbps 単位で加算
            score += (ni.Speed / 1_000_000.0);

            // APIPA (169.254.x.x) の場合は大幅にペナルティを与える
            var ipProps = ni.GetIPProperties();
            var uniCastInfo = ipProps.UnicastAddresses
                .FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork);
            if (uniCastInfo == null) return score;
            var ip = uniCastInfo.Address.ToString();
            if (ip.StartsWith("169.254"))
            {
                score -= 10000;
            }

            return score;
        }
    }
}
