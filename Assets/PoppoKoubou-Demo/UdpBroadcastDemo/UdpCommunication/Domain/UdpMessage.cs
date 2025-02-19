// ReSharper disable MemberCanBePrivate.Global
namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Domain
{
    public class UdpMessage
    {
        public readonly string Text;
        public UdpMessage(string text) => Text = text;
        public static UdpMessage Create(string text) => new UdpMessage(text);
    }
}
