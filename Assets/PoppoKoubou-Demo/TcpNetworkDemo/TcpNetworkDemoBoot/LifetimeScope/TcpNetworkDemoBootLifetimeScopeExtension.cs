using MessagePipe;
using PoppoKoubou_Demo.TcpNetworkDemo.TcpNetworkDemoBoot.Application;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.TcpNetworkDemo.TcpNetworkDemoBoot.LifetimeScope
{
    public static class TcpNetworkDemoBootLifetimeScopeExtension
    {
        public static void AddTcpNetworkDemoBootMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddTcpNetworkDemoBootComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
        }
        public static void AddTcpNetworkDemoBootEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TcpNetworkDemoBootService>();
        }
    }
}