using MessagePipe;
using PoppoKoubou_Demo.LogDemo.LogDemoBoot.Application;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou_Demo.LogDemo.LogDemoBoot.LifetimeScope
{
    public static class LogDemoLifetimeScopeExtension
    {
        public static void AddLogDemoBootMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
        }
        public static void AddLogDemoBootComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
        }
        public static void AddLogDemoBootEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LogDemoBootService>();
        }
    }
}