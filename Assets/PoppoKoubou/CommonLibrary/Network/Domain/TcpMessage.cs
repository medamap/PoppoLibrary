using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePipe.Interprocess;

// ReSharper disable MemberCanBePrivate.Global

namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    [MessagePackObject] public readonly struct TcpMessage : IToEndpointable
    {
        [Key(0)] public readonly string FromAddress { get; }
        [Key(1)] public readonly string ToAddress { get; }
        [Key(2)] public readonly int ToPort { get; }
        [Key(3)] public readonly string Text;
        public TcpMessage(string fromAddress, string toAddress, int toPort, string text)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            ToPort = toPort;
            Text = text;
        }
        public string GetToAddress() => ToAddress;
        public int GetPort() => ToPort;
        public static TcpMessage Create(string fromAddress, string toAddress, int toPort, string text)
            => new TcpMessage(fromAddress, toAddress, toPort, text);
    }
    
    public class TcpMessageFormatter : IMessagePackFormatter<TcpMessage>
    {
        public TcpMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            int count = reader.ReadArrayHeader();
            if (count != 4)
            {
                throw new InvalidOperationException("Invalid array length for TcpMessage");
            }
            string fromAddress = reader.ReadString();
            string toAddress = reader.ReadString();
            int toPort = reader.ReadInt32();
            string text = reader.ReadString();
            return new TcpMessage(fromAddress, toAddress, toPort, text);
        }

        public void Serialize(ref MessagePackWriter writer, TcpMessage value, MessagePackSerializerOptions options)
        {
            // UdpMessage を 4 要素の配列としてシリアライズする
            writer.WriteArrayHeader(4);
            writer.Write(value.FromAddress);
            writer.Write(value.ToAddress);
            writer.Write(value.ToPort);
            writer.Write(value.Text);
        }
    }
}