﻿using UnityEngine;

namespace PoppoKoubou.CommonLibrary.UI.Domain
{
    /// <summary>UIインタラクトメッセージ</summary>
    public struct InteractUI
    {
        public InteractUIType Type { get; }
        public GameObject Sender { get; }
        public string Message { get; }
        private InteractUI(InteractUIType type, GameObject sender, string message = null)
        {
            Type = type;
            Sender = sender;
            Message = message;
        }
        public static InteractUI ClickButton(GameObject sender, string message = null) => new (InteractUIType.ClickButton, sender, message);
    }
}