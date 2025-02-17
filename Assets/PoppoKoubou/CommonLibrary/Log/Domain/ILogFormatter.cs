namespace PoppoKoubou.CommonLibrary.Log.Domain
{
    public interface ILogFormatter
    {
        string Format(LogMessage message);
    }
}