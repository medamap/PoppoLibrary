using UnityEditor;

namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービスノード情報</summary>
    public struct ServiceNodeInfo
    {
        /// <summary>GUID</summary>
        public System.Guid Guid { get; }
        /// <summary>サービスノード名</summary>
        public string Name { get; }
        /// <summary>実行優先順位（小さいほど優先度が高い）</summary>
        public int Priority { get; }
        /// <summary>コンストラクタ</summary>
        public ServiceNodeInfo(string name, int priority)
        {
            Name = name;
            Priority = priority;
            Guid = System.Guid.NewGuid();
        }
    }
}
