using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.Application;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpCommunication.LifetimeScope
{
    public static class UdpCommunicationLifetimeScopeExtension
    {
        public static void AddUdpCommunicationMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddUdpCommunicationComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
        }
        public static void AddUdpCommunicationEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<UdpCommunicationService>();
        }
    }
}