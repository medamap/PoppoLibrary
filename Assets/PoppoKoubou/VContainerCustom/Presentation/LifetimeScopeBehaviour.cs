using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.AggregateService.LifetimeScope;
using PoppoKoubou.CommonLibrary.Log.Domain;
using PoppoKoubou.CommonLibrary.Log.LifetimeScope;
using PoppoKoubou.CommonLibrary.Network.LifetimeScope;
using PoppoKoubou.CommonLibrary.UI.LifetimeScope;
using VContainer;
using VContainer.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace PoppoKoubou.VContainerCustom.Presentation
{
    public abstract class LifetimeScopeBehaviour : LifetimeScope 
    {
        [Flags] protected enum Services
        {
            None             = 0,
            Log              = 1 << 0,
            UI               = 1 << 1,
            Network          = 1 << 2,
            All              = Log | UI | Network,
            NonNetwork       = Log | UI
        }
        protected IPublisher<LogMessage> LogPublisher = null;
        protected Services AvailableServices  { get; set; }
        protected MessagePipeOptions MessagePipeOptions { get; private set; }
        protected override void Configure(IContainerBuilder builder)
        {
            // Initialize ////////////////////////////////////////
            AvailableServices = Services.All;
            var options = MessagePipeOptions = builder.RegisterMessagePipe();
            OnInitialize();

            // Message ////////////////////////////////////////
            this.AddAggregateServiceMessage(builder, options);
            if (AvailableServices.HasFlag(Services.Log))
            {
                this.AddLogMessage(builder, options);
                Container?.TryResolve(out LogPublisher);
            }
            if (AvailableServices.HasFlag(Services.UI)) this.AddUIMessage(builder, options);
            if (AvailableServices.HasFlag(Services.Network)) this.AddNetworkMessage(builder, options);
            OnRegisterMessage(builder, options);

            // Component ////////////////////////////////////////
            this.AddAggregateServiceComponent(builder);
            if (AvailableServices.HasFlag(Services.Log)) this.AddLogComponent(builder);
            if (AvailableServices.HasFlag(Services.UI)) this.AddUIComponent(builder);
            if (AvailableServices.HasFlag(Services.Network)) this.AddNetworkComponent(builder);
            OnRegisterComponent(builder);
            
            // EntryPoint ////////////////////////////////////////
            this.AddAggregateServiceEntryPoint(builder);
            if (AvailableServices.HasFlag(Services.Log)) this.AddLogEntryPoint(builder);
            if (AvailableServices.HasFlag(Services.UI)) this.AddUIEntryPoint(builder);
            if (AvailableServices.HasFlag(Services.Network)) this.AddNetworkEntryPoint(builder);
            OnRegisterEntryPoint(builder);
        }
        protected abstract void OnInitialize();
        protected abstract void OnRegisterMessage(IContainerBuilder builder, MessagePipeOptions options);
        protected abstract void OnRegisterComponent(IContainerBuilder builder);
        protected abstract void OnRegisterEntryPoint(IContainerBuilder builder);
        protected abstract void OnDispose();
        protected override void OnDestroy()
        {
            OnDispose();
            base.OnDestroy();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(LifetimeScopeBehaviour), true)]
    public class CustomLifetimeScopeBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
#endif

}