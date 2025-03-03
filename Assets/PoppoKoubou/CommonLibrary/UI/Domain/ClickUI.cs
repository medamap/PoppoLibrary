using System;
using UnityEngine;
using MessagePack;
using MessagePack.Formatters;
using UnityEngine.UI;

namespace PoppoKoubou.CommonLibrary.UI.Domain
{
    [MessagePackObject] public struct ClickUI
    {
        [Key(0)] public GameObject Sender { get; }
        [Key(1)] public Vector2 Position { get; }
        [Key(2)] public string Message { get; }

        public ClickUI(GameObject sender, Vector2 position, string message = null)
        {
            Sender = sender;
            Position = position;
            Message = message;
        }
    }
    
    // ClickUI 用のカスタムフォーマッター
    public class ClickUIFormatter : IMessagePackFormatter<ClickUI>
    {
        public ClickUI Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // ClickUI は 3 要素の配列としてシリアライズされている前提
            int count = reader.ReadArrayHeader();
            if (count != 3)
            {
                throw new InvalidOperationException("Invalid array length for ClickUI");
            }

            // Sender はカスタム文字列としてシリアライズ
            string senderStr = reader.ReadString();
            GameObject sender = null;

            string[] parts = senderStr.Split('|');
            if (parts.Length > 0)
            {
                string uiType = parts[0];
                string name = parts.Length > 1 ? parts[1] : "Unnamed";
                sender = new GameObject(name);

                if (uiType == "Button")
                {
                    if (sender.GetComponent<Button>() == null)
                    {
                        sender.AddComponent<Button>();
                    }
                }
                else if (uiType == "Toggle")
                {
                    Toggle toggle = sender.AddComponent<Toggle>();
                    if (parts.Length > 2 && bool.TryParse(parts[2], out bool isOn))
                    {
                        toggle.isOn = isOn;
                    }
                }
                // その他の UI 要素には対応せず、"Empty" はそのまま
            }
            else
            {
                sender = new GameObject("Unnamed");
            }

            // Position は Vector2
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Vector2 position = new Vector2(x, y);

            string message = reader.ReadString();
            return new ClickUI(sender, position, message);
        }

        public void Serialize(ref MessagePackWriter writer, ClickUI value, MessagePackSerializerOptions options)
        {
            // ClickUI を 3 要素の配列としてシリアライズする
            writer.WriteArrayHeader(3);

            string senderStr = "";
            if (value.Sender != null)
            {
                if (value.Sender.GetComponent<Button>() != null)
                {
                    senderStr = "Button|" + value.Sender.name;
                }
                else if (value.Sender.GetComponent<Toggle>() != null)
                {
                    Toggle toggle = value.Sender.GetComponent<Toggle>();
                    senderStr = "Toggle|" + value.Sender.name + "|" + toggle.isOn;
                }
                else
                {
                    senderStr = "Empty|" + value.Sender.name;
                }
            }
            else
            {
                senderStr = "Empty|Unnamed";
            }
            writer.Write(senderStr);

            // Vector2 の x, y を順にシリアライズ
            writer.Write(value.Position.x);
            writer.Write(value.Position.y);

            writer.Write(value.Message);
        }
    }
}