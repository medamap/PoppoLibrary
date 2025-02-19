using System.Collections;
using System.Collections.Generic;

namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    public class NetworkInfo
    {
        public string LocalIPAddress { get; }
        public string DefaultGateway { get; }
        public string SubnetMask { get; }
        public string NetworkAddress { get; }
        public string BroadcastAddress { get; }
        public List<string> LOG = new List<string>();
        public bool IsError => LOG.Count > 0;
        
        public NetworkInfo(string localIPAddress, string defaultGateway, string subnetMask, string networkAddress, string broadcastAddress)
        {
            LocalIPAddress = localIPAddress;
            DefaultGateway = defaultGateway;
            SubnetMask = subnetMask;
            NetworkAddress = networkAddress;
            BroadcastAddress = broadcastAddress;
        }
        public NetworkInfo(List<string> log)
        {
            LOG.AddRange(log);
        }
        public void AddLog(List<string> log)
        {
            LOG.AddRange(log);
        }
    }
}