using PoppoKoubou.CommonLibrary.AggregateService.Infrastructure;

namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービスノードステータス</summary>
    public struct ServiceNodeStatus
    {
        /// <summary>サービスノード</summary>
        public ServiceNode ServiceNode { get; }
        /// <summary>サービスノード情報</summary>
        public ServiceNodeInfo ServiceNodeInfo { get; }
        /// <summary>フェイズ</summary>
        public ServiceNodeStatusPhase Phase { get; }
        /// <summary>コンストラクタ</summary>
        public ServiceNodeStatus(ServiceNodeInfo serviceNodeInfo, ServiceNodeStatusPhase phase, ServiceNode node = null)
        {
            ServiceNode = node;
            ServiceNodeInfo = serviceNodeInfo;
            Phase = phase;
        }
        /// <summary>サービスノード登録</summary>
        public static ServiceNodeStatus RegistrationServiceNode(ServiceNodeInfo serviceNodeInfo, ServiceNode serviceNode = null) =>
            new (serviceNodeInfo, ServiceNodeStatusPhase.RegistrationServiceNode, serviceNode);
        
        /// <summary>サービスノード初期化開始</summary>
        public static ServiceNodeStatus StartInitializeServiceNode(ServiceNodeInfo serviceNodeInfo, ServiceNode serviceNode = null) =>
            new (serviceNodeInfo, ServiceNodeStatusPhase.StartInitializeServiceNode, serviceNode);

        /// <summary>サービスノード初期化完了</summary>
        public static ServiceNodeStatus CompleteInitializeServiceNode(ServiceNodeInfo serviceNodeInfo, ServiceNode serviceNode = null) =>
            new (serviceNodeInfo, ServiceNodeStatusPhase.CompleteInitializeServiceNode, serviceNode);
        
        /// <summary>サービスノード開始</summary>
        public static ServiceNodeStatus StartServiceNode(ServiceNodeInfo serviceNodeInfo, ServiceNode serviceNode = null) =>
            new (serviceNodeInfo, ServiceNodeStatusPhase.StartServiceNode, serviceNode);
    }
}