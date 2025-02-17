namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービス集約ハブステータス</summary>
    public struct CentralHubStatus
    {
        /// <summary>フェイズ</summary>
        public CentralHubStatusPhase Phase { get; }
        /// <summary>サービスノード優先順位</summary>
        public float Priority { get; }
        /// <summary>コンストラクタ</summary>
        public CentralHubStatus(CentralHubStatusPhase phase, float priority)
        {
            Phase = phase;
            Priority = priority;
        }
        
        /// <summary>サービスノード登録待ち</summary>
        public static CentralHubStatus WaitingRegistrationServiceNode() =>
            new (CentralHubStatusPhase.WaitingRegistrationServiceNode, 0f);
        
        /// <summary>サービスノード初期化待ち</summary>
        public static CentralHubStatus WaitingInitializeServiceNode(float priority) =>
            new (CentralHubStatusPhase.WaitingInitializeServiceNode, priority);
        
        /// <summary>サービスノード開始許可</summary>
        public static CentralHubStatus AllowStartServiceNode() =>
            new (CentralHubStatusPhase.AllowStartServiceNode, 0f);
    }
}
