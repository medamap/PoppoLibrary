using System.Collections.Generic;
using PoppoKoubou.CommonLibrary.Network.Domain;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace PoppoKoubou.CommonLibrary.Network.Infrastructure
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NetworkInfoProvider : INetworkInfoProvider
    {
        /// <summary>依存注入</summary>
        [Inject]
        public NetworkInfoProvider() { }
        
        /// <summary>非同期ネットワーク情報取得</summary>
        public async UniTask<NetworkInfo> GetNetworkInfoAsync(CancellationToken token)
        {
            return await UniTask.RunOnThreadPool(() => GetNetworkInfo(token), cancellationToken: token);
        }

        /// <summary>ネットワーク情報取得</summary>
        public NetworkInfo GetNetworkInfo(CancellationToken token = default)
        {
            // キャンセルチェック
            if (!token.Equals(default)) token.ThrowIfCancellationRequested();
            
            var logs = new List<string>();
            logs.Add("GetNetworkInfo()");

            // すべてのインターフェース名を取得してログに記録
            var allInterfaceNames = new List<string>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                allInterfaceNames.Add(ni.Name);
            }
            logs.Add($"All Interfaces: {string.Join(", ", allInterfaceNames)}");

            // 稼働中かつループバック以外、かつ名前が "dummy" で始まらないインターフェースを取得
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                             !ni.Name.StartsWith("dummy", System.StringComparison.OrdinalIgnoreCase));

            logs.Add($"Valid Interface Count: {interfaces.Count()}");
            
            // 各インターフェースにスコアを付与して、最もスコアの高い候補を選択
            var candidate = interfaces
                .Select(ni => new { Interface = ni, Score = ComputeInterfaceScore(ni) })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            logs.Add($"Selected Interface : {candidate?.Interface.Name ?? "None"}");
            
            if (candidate == null)
            {
                logs.Add("No valid interface candidate found. Available interfaces: " + string.Join(", ", allInterfaceNames));
                return new NetworkInfo(logs);
            }

            logs.Add($"Selected Interface Score : {candidate.Score}");
            
            // キャンセルチェック
            if (!token.Equals(default)) token.ThrowIfCancellationRequested();

            var selectedInterface = candidate.Interface;
            var ipProps = selectedInterface.GetIPProperties();

            logs.Add($"Selected Interface Description: {selectedInterface.Description}");
            
            // IPv4 のデフォルトゲートウェイ（ルータIP）を取得
            var gatewayAddress = ipProps.GatewayAddresses
                .FirstOrDefault(g => g.Address.AddressFamily == AddressFamily.InterNetwork)?.Address;
            string gateway = "";
            if (gatewayAddress == null)
            {
                logs.Add("No valid IPv4 Gateway found. Gateway will be empty.");
            }
            else
            {
                gateway = gatewayAddress.ToString();
                logs.Add($"Gateway : {gateway}");
            }
            
            // IPv4 の Unicast アドレス情報（IPv4Mask が取得できるもの）を取得
            var unicastInfo = ipProps.UnicastAddresses
                .FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork && u.IPv4Mask != null);
            if (unicastInfo == null)
            {
                logs.Add("No valid IPv4 Unicast address found.");
                return new NetworkInfo(logs);
            }
            
            logs.Add($"Local IP : {unicastInfo.Address}");
            
            // キャンセルチェック
            if (!token.Equals(default)) token.ThrowIfCancellationRequested();

            var localIP = unicastInfo.Address;
            var subnetMask = unicastInfo.IPv4Mask;
            logs.Add($"Subnet Mask : {subnetMask}");
            
            // ネットワークアドレス = ローカルIP と サブネットマスクの AND
            var ipBytes = localIP.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            if (ipBytes.Length != maskBytes.Length)
            {
                logs.Add("IP address bytes length does not match subnet mask bytes length.");
                return new NetworkInfo(logs);
            }
            
            logs.Add($"IP Bytes : {string.Join(".", ipBytes)}");
            
            var networkBytes = new byte[ipBytes.Length];
            for (var i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }
            var networkAddress = new IPAddress(networkBytes);
            logs.Add($"Network Address : {networkAddress}");
            
            // ブロードキャストアドレス = ネットワークアドレス OR (NOT サブネットマスク)
            var broadcastBytes = new byte[ipBytes.Length];
            for (var i = 0; i < ipBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(networkBytes[i] | (~maskBytes[i]));
            }
            var broadcastAddress = new IPAddress(broadcastBytes);
            logs.Add($"Broadcast Address : {broadcastAddress}");

            return new NetworkInfo(
                localIP.ToString(),
                gateway,
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
