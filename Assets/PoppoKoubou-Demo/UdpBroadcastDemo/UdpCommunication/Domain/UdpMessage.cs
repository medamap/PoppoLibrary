using MessagePack;
using MessagePack.Formatters;
using System;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Domain
{
    [MessagePackObject]
    public class UdpMessage
    {
        [Key(0)]
        public readonly string Text;
        public UdpMessage(string text) => Text = text;
        public static UdpMessage Create(string text) => new UdpMessage(text);
    }

    // UdpMessage 用のカスタムフォーマッター
    public class UdpMessageFormatter : IMessagePackFormatter<UdpMessage>
    {
        public UdpMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // UdpMessage を 1 要素の配列としてシリアライズする前提
            int count = reader.ReadArrayHeader();
            if (count != 1)
            {
                throw new InvalidOperationException("Invalid array length for UdpMessage");
            }
            string text = reader.ReadString();
            return new UdpMessage(text);
        }

        public void Serialize(ref MessagePackWriter writer, UdpMessage value, MessagePackSerializerOptions options)
        {
            // UdpMessage を 1 要素の配列としてシリアライズする
            writer.WriteArrayHeader(1);
            writer.Write(value.Text);
        }
    }
}