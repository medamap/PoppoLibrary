namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    public enum ServiceNodeStatusPhase
    {
        /// <summary>サービスノード登録</summary>
        RegistrationServiceNode,
        /// <summary>サービスノード初期化開始</summary>
        StartInitializeServiceNode,
        /// <summary>サービスノード初期化完了</summary>
        CompleteInitializeServiceNode,
        /// <summary>サービスノード開始</summary>
        StartServiceNode,
    }
}
