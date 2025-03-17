using MessagePipe;
using PoppoKoubou_Demo.UdpBroadcastDemo.UdpBroadCastDemoBoot.Application;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.UdpBroadcastDemo.UdpBroadCastDemoBoot.LifetimeScope
{
    public static class UdpBroadcastDemoBootLifetimeScopeExtension
    {
        public static void AddUdpBroadcastDemoBootMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddUdpBroadcastDemoBootComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
        }
        public static void AddUdpBroadcastDemoBootEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<UdpBroadcastDemoBootService>();
        }
    }
}