using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;

namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    [MessagePackObject] public class NetworkInfo
    {
        [Key(0)] public string LocalIPAddress { get; }
        [Key(1)] public string DefaultGateway { get; }
        [Key(2)] public string SubnetMask { get; }
        [Key(3)] public string NetworkAddress { get; }
        [Key(4)] public string BroadcastAddress { get; }
        [Key(5)] public readonly List<string> LOG = new List<string>();
        [IgnoreMember] public bool IsError => LOG.Count > 0;
        
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
    
    public class NetworkInfoFormatter : IMessagePackFormatter<NetworkInfo>
    {
        public NetworkInfo Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // NetworkInfo は 6 要素の配列としてシリアライズされている前提です
            int count = reader.ReadArrayHeader();
            if (count != 6)
            {
                throw new InvalidOperationException("Invalid array length for NetworkInfo");
            }
            
            string localIPAddress = reader.ReadString();
            string defaultGateway = reader.ReadString();
            string subnetMask = reader.ReadString();
            string networkAddress = reader.ReadString();
            string broadcastAddress = reader.ReadString();
            
            // LOG は List<string> としてシリアライズされている
            List<string> log = options.Resolver.GetFormatterWithVerify<List<string>>().Deserialize(ref reader, options);
            
            // まずアドレス情報からインスタンスを生成
            NetworkInfo instance = new NetworkInfo(localIPAddress, defaultGateway, subnetMask, networkAddress, broadcastAddress);
            
            // LOG にデータがある場合、AddLog で追加
            if (log != null && log.Count > 0)
            {
                instance.AddLog(log);
            }
            
            return instance;
        }

        public void Serialize(ref MessagePackWriter writer, NetworkInfo value, MessagePackSerializerOptions options)
        {
            // 6 要素の配列としてシリアライズする
            writer.WriteArrayHeader(6);
            writer.Write(value.LocalIPAddress);
            writer.Write(value.DefaultGateway);
            writer.Write(value.SubnetMask);
            writer.Write(value.NetworkAddress);
            writer.Write(value.BroadcastAddress);
            options.Resolver.GetFormatterWithVerify<List<string>>().Serialize(ref writer, value.LOG, options);
        }
    }
}