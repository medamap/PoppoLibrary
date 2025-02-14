namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービス集約ハブステータスフェイズ</summary>
    public enum CentralHubStatusPhase
    {
        /// <summary>サービスノード登録待ち</summary>
        WaitingRegistrationServiceNode,
        /// <summary>サービスノード初期化待ち</summary>
        WaitingInitializeServiceNode,
        /// <summary>サービスノード開始許可</summary>
        AllowStartServiceNode,
    }
}
