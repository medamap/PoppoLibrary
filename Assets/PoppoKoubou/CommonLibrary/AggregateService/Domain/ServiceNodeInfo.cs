using System;
using System.Buffers;
using MessagePack;
using MessagePack.Formatters;

namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>
    /// サービスノード情報
    /// </summary>
    [MessagePackObject] public struct ServiceNodeInfo
    {
        /// <summary>GUID</summary>
        [Key(0)] public Guid Guid { get; }
        /// <summary>サービスノード名</summary>
        [Key(1)] public string Name { get; }
        /// <summary>実行優先順位（小さいほど優先度が高い）</summary>
        [Key(2)] public int Priority { get; }
        
        /// <summary>コンストラクタ</summary>
        public ServiceNodeInfo(string name, int priority)
        {
            Name = name;
            Priority = priority;
            Guid = Guid.NewGuid();
        }

        public ServiceNodeInfo(string name, int priority, Guid guid)
        {
            Name = name;
            Priority = priority;
            Guid = guid;
        }
    }

    // ServiceNodeInfo 用のカスタムフォーマッター
    public class ServiceNodeInfoFormatter : IMessagePackFormatter<ServiceNodeInfo>
    {
        public ServiceNodeInfo Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // 3 要素の配列としてシリアライズされている前提
            int count = reader.ReadArrayHeader();
            if (count != 3)
            {
                throw new InvalidOperationException("Invalid array length for ServiceNodeInfo");
            }
            
            // GUID を byte[] として読み込み、Guid 型へ復元
            var guidBytes = reader.ReadBytes();
            if (!guidBytes.HasValue)
            {
                throw new InvalidOperationException("Guid bytes is null");
            }
            Guid guid = new Guid(guidBytes.Value.ToArray());
            
            string name = reader.ReadString();
            int priority = reader.ReadInt32();
            
            return new ServiceNodeInfo(name, priority, guid);
        }

        public void Serialize(ref MessagePackWriter writer, ServiceNodeInfo value, MessagePackSerializerOptions options)
        {
            // 3 要素の配列としてシリアライズする
            writer.WriteArrayHeader(3);
            writer.Write(value.Guid.ToByteArray());
            writer.Write(value.Name);
            writer.Write(value.Priority);
        }
    }
}
