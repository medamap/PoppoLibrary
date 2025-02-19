using MessagePipe;
using PoppoKoubou.CommonLibrary.Network.Application;
using PoppoKoubou.CommonLibrary.Network.Domain;
using PoppoKoubou.CommonLibrary.Network.Infrastructure;
using VContainer;
using VContainer.Unity;

namespace PoppoKoubou.CommonLibrary.Network.LifetimeScope
{
    public static class NetworkLifetimeScopeExtensions
    {
        public static void AddNetworkMessage(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder, MessagePipeOptions options) {
            builder.RegisterMessageBroker<NetworkInfo>(options);
        }
        public static void AddNetworkComponent(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
            builder.Register<INetworkInfoProvider, NetworkInfoProvider>(Lifetime.Singleton);
            builder.Register<INetworkInfoContainer, NetworkInfoContainer>(Lifetime.Singleton);
        }
        public static void AddNetworkEntryPoint(this VContainer.Unity.LifetimeScope lifetimeScope, IContainerBuilder builder) {
            builder.RegisterEntryPoint<NetworkService>();
        }
    }
}