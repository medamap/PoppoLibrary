using System.Collections;

namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    public class NetworkInfo
    {
        public string LocalIPAddress { get; }
        public string DefaultGateway { get; }
        public string SubnetMask { get; }
        public string NetworkAddress { get; }
        public string BroadcastAddress { get; }
        
        public NetworkInfo(string localIPAddress, string defaultGateway, string subnetMask, string networkAddress, string broadcastAddress)
        {
            LocalIPAddress = localIPAddress;
            DefaultGateway = defaultGateway;
            SubnetMask = subnetMask;
            NetworkAddress = networkAddress;
            BroadcastAddress = broadcastAddress;
        }
    }
}