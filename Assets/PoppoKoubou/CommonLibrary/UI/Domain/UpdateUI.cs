namespace PoppoKoubou.CommonLibrary.UI.Domain
{
    public struct UpdateUI
    {
        public UpdateUIType Type { get; }
        public string ValueString { get; }
        public int ValueInt { get; }
        public float ValueFloat { get; }
        
        private UpdateUI(UpdateUIType type, string valueString = null, int valueInt = 0, float valueFloat = 0)
        {
            Type = type;
            this.ValueString = valueString;
            this.ValueInt = valueInt;
            this.ValueFloat = valueFloat;
        }
        
        public static UpdateUI UpdateVerticalSize(float value) => new (UpdateUIType.UpdateVerticalSize, null, 0, value);
    }
}