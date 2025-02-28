using System;
using MessagePack;
using MessagePack.Formatters;
using UnityEngine;
using UnityEngine.UI;

namespace PoppoKoubou.CommonLibrary.UI.Domain
{
    /// <summary>UIインタラクトメッセージ</summary>
    [MessagePackObject] public struct InteractUI
    {
        [Key(0)] public InteractUIType Type { get; }
        [Key(1)] public GameObject Sender { get; }
        [Key(2)] public string Message { get; }

        public InteractUI(InteractUIType type, GameObject sender, string message = null)
        {
            Type = type;
            Sender = sender;
            Message = message;
        }

        public static InteractUI ClickButton(GameObject sender, string message = null)
            => new InteractUI(InteractUIType.ClickButton, sender, message);
    }

    // InteractUI 用のカスタムフォーマッター
    public class InteractUIFormatter : IMessagePackFormatter<InteractUI>
    {
        public InteractUI Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // InteractUI は 3 要素の配列としてシリアライズされている前提
            int count = reader.ReadArrayHeader();
            if (count != 3)
            {
                throw new InvalidOperationException("Invalid array length for InteractUI");
            }
            
            // Type は enum として int でシリアライズしている前提
            int typeInt = reader.ReadInt32();
            InteractUIType type = (InteractUIType)typeInt;
            
            // Sender はカスタム文字列としてシリアライズされている
            string senderStr = reader.ReadString();
            GameObject sender = null;
            // 文字列は "UIType|Name" もしくは "UIType|Name|Extra" の形式を想定
            string[] parts = senderStr.Split('|');
            if (parts.Length > 0)
            {
                string uiType = parts[0];
                string name = parts.Length > 1 ? parts[1] : "Unnamed";
                // 送信側で UI が１つだけアタッチされている前提で、受信側では新規に GameObject を生成してアタッチする
                sender = new GameObject(name);
                if (uiType == "Button")
                {
                    // Button をアタッチ
                    if (sender.GetComponent<Button>() == null)
                    {
                        sender.AddComponent<Button>();
                    }
                }
                else if (uiType == "Toggle")
                {
                    // Toggle をアタッチし、isOn 状態を復元
                    Toggle toggle = sender.AddComponent<Toggle>();
                    if (parts.Length > 2 && bool.TryParse(parts[2], out bool isOn))
                    {
                        toggle.isOn = isOn;
                    }
                }
                // "Empty" もしくは不明な場合はそのまま（空の GameObject として）
            }
            else
            {
                // 文字列が空の場合はデフォルトの GameObject を生成
                sender = new GameObject("Unnamed");
            }
            
            string message = reader.ReadString();
            return new InteractUI(type, sender, message);
        }

        public void Serialize(ref MessagePackWriter writer, InteractUI value, MessagePackSerializerOptions options)
        {
            // InteractUI を 3 要素の配列としてシリアライズする
            writer.WriteArrayHeader(3);
            writer.Write((int)value.Type);
            
            string senderStr = "";
            if (value.Sender != null)
            {
                // 送信側の GameObject にアタッチされている主要な UI 要素をチェック
                if (value.Sender.GetComponent<Button>() != null)
                {
                    // Button の場合: "Button|{GameObject名}"
                    senderStr = "Button|" + value.Sender.name;
                }
                else if (value.Sender.GetComponent<Toggle>() != null)
                {
                    // Toggle の場合: "Toggle|{GameObject名}|{isOn}"
                    Toggle toggle = value.Sender.GetComponent<Toggle>();
                    senderStr = "Toggle|" + value.Sender.name + "|" + toggle.isOn;
                }
                else
                {
                    // それ以外は空の GameObject として扱う: "Empty|{GameObject名}"
                    senderStr = "Empty|" + value.Sender.name;
                }
            }
            else
            {
                senderStr = "Empty|Unnamed";
            }
            writer.Write(senderStr);
            writer.Write(value.Message);
        }
    }
}
