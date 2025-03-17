using MessagePipe;
using PoppoKoubou_Demo.TcpNetworkDemo.TcpCommunication.Application;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.TcpNetworkDemo.TcpCommunication.LifetimeScope
{
    public static class TcpCommunicationLifetimeScopeExtension
    {
        public static void AddTcpCommunicationMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddTcpCommunicationComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
        }
        public static void AddTcpCommunicationEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TcpCommunicationService>();
        }
    }
}