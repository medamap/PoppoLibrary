using System;
using MessagePack;
using MessagePack.Formatters;

namespace PoppoKoubou.CommonLibrary.UI.Domain
{
    [MessagePackObject] public struct UpdateUI
    {
        [Key(0)] public UpdateUIType Type { get; }
        [Key(1)] public string ValueString { get; }
        [Key(2)] public int ValueInt { get; }
        [Key(3)] public float ValueFloat { get; }
        
        public UpdateUI(UpdateUIType type, string valueString = null, int valueInt = 0, float valueFloat = 0)
        {
            Type = type;
            ValueString = valueString;
            ValueInt = valueInt;
            ValueFloat = valueFloat;
        }
        
        public static UpdateUI UpdateVerticalSize(float value) => new(UpdateUIType.UpdateVerticalSize, null, 0, value);
    }

    // UpdateUI 用のカスタムフォーマッター
    public class UpdateUIFormatter : IMessagePackFormatter<UpdateUI>
    {
        public UpdateUI Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // UpdateUI は 4 要素の配列としてシリアライズされている前提
            int count = reader.ReadArrayHeader();
            if (count != 4)
            {
                throw new InvalidOperationException("Invalid array length for UpdateUI");
            }
            
            int typeInt = reader.ReadInt32();
            UpdateUIType type = (UpdateUIType)typeInt;
            string valueString = reader.ReadString();
            int valueInt = reader.ReadInt32();
            float valueFloat = reader.ReadSingle();
            
            return new UpdateUI(type, valueString, valueInt, valueFloat);
        }

        public void Serialize(ref MessagePackWriter writer, UpdateUI value, MessagePackSerializerOptions options)
        {
            // 4 要素の配列としてシリアライズする
            writer.WriteArrayHeader(4);
            writer.Write((int)value.Type);
            writer.Write(value.ValueString);
            writer.Write(value.ValueInt);
            writer.Write(value.ValueFloat);
        }
    }
}
