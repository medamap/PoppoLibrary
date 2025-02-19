using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Application;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.Log.LifetimeScope
{
    public static class LogLifetimeScopeExtensions
    {
        public static void AddLogMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
            builder.RegisterMessageBroker<LogMessage>(options);
        }
        public static void AddLogComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder)
        {
            builder.Register<ILogFormatter, UnityLogFormatter>(Lifetime.Singleton);
            builder.Register<ILogOperator, TextMeshProUguiLogOperator>(Lifetime.Transient);
            builder.Register<ILogProvider, UnityLogProvider>(Lifetime.Singleton);
        }
        public static void AddLogEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
            builder.RegisterEntryPoint<LogService>();
        }
    }
}
