namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービスノードステータス</summary>
    public struct ServiceNodeStatus
    {
        /// <summary>サービスノード情報</summary>
        public ServiceNodeInfo ServiceNodeInfo { get; }
        /// <summary>フェイズ</summary>
        public ServiceNodeStatusPhase Phase { get; }
        /// <summary>コンストラクタ</summary>
        public ServiceNodeStatus(ServiceNodeInfo serviceNodeInfo, ServiceNodeStatusPhase phase)
        {
            ServiceNodeInfo = serviceNodeInfo;
            Phase = phase;
        }
        /// <summary>サービスノード登録</summary>
        public static ServiceNodeStatus RegistrationServiceNode(ServiceNodeInfo serviceNodeInfo) =>
            new ServiceNodeStatus(serviceNodeInfo, ServiceNodeStatusPhase.RegistrationServiceNode);
        
        /// <summary>サービスノード初期化開始</summary>
        public static ServiceNodeStatus StartInitializeServiceNode(ServiceNodeInfo serviceNodeInfo) =>
            new ServiceNodeStatus(serviceNodeInfo, ServiceNodeStatusPhase.StartInitializeServiceNode);

        /// <summary>サービスノード初期化完了</summary>
        public static ServiceNodeStatus CompleteInitializeServiceNode(ServiceNodeInfo serviceNodeInfo) =>
            new ServiceNodeStatus(serviceNodeInfo, ServiceNodeStatusPhase.CompleteInitializeServiceNode);
        
        /// <summary>サービスノード開始</summary>
        public static ServiceNodeStatus StartServiceNode(ServiceNodeInfo serviceNodeInfo) =>
            new ServiceNodeStatus(serviceNodeInfo, ServiceNodeStatusPhase.StartServiceNode);
    }
}