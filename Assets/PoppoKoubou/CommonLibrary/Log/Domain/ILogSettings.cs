namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    public interface ILogSettings
    {
        LogLevel LogLevel { get; }
        public void UpdateLogSettings(LogLevel logLevel);
    }
}