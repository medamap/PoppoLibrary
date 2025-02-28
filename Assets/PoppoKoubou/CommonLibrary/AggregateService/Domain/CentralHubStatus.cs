using System;
using MessagePack;
using MessagePack.Formatters;

namespace PoppoKoubou.CommonLibrary.AggregateService.Domain
{
    /// <summary>サービス集約ハブステータス</summary>
    [MessagePackObject] public struct CentralHubStatus
    {
        /// <summary>フェイズ</summary>
        [Key(0)] public CentralHubStatusPhase Phase { get; }
        /// <summary>サービスノード優先順位</summary>
        [Key(1)] public float Priority { get; }

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

    // CentralHubStatus 用のカスタムフォーマッター
    public class CentralHubStatusFormatter : IMessagePackFormatter<CentralHubStatus>
    {
        public CentralHubStatus Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // CentralHubStatus は 2 要素の配列としてシリアライズされている前提です
            int count = reader.ReadArrayHeader();
            if (count != 2)
            {
                throw new InvalidOperationException("Invalid array length for CentralHubStatus");
            }
            
            // Phase (enum) は int としてシリアライズされている前提
            int phaseInt = reader.ReadInt32();
            CentralHubStatusPhase phase = (CentralHubStatusPhase)phaseInt;
            
            // Priority
            float priority = reader.ReadSingle();
            
            return new CentralHubStatus(phase, priority);
        }

        public void Serialize(ref MessagePackWriter writer, CentralHubStatus value, MessagePackSerializerOptions options)
        {
            // 2 要素の配列としてシリアライズする
            writer.WriteArrayHeader(2);
            writer.Write((int)value.Phase);
            writer.Write(value.Priority);
        }
    }
}
