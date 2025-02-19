namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    /// <summary>ネットワークサービス情報コンテナ</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NetworkInfoContainer : INetworkInfoContainer
    {
        /// <summary>ネットワークサービス情報</summary>
        public NetworkInfo NetworkInfo { get; private set; }
        
        /// <summary>コンストラクタ</summary>
        public NetworkInfoContainer()
            => NetworkInfo = new NetworkInfo(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        
        /// <summary>ネットワーク情報設定</summary>
        public void SetNetworkInfo(NetworkInfo networkInfo) => NetworkInfo = networkInfo;
    }
}