namespace PoppoKoubou.CommonLibrary.Network.Domain
{
    public interface INetworkInfoContainer
    {
        /// <summary>ネットワークサービス情報</summary>
        public NetworkInfo NetworkInfo { get; }
        /// <summary>ネットワーク情報設定</summary>
        public void SetNetworkInfo(NetworkInfo networkInfo);
    }
}