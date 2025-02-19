using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Domain;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    public static class LogExtensions
    {
        public static void AddLine(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
            => publisher.Publish(LogMessage.AddLine(message, level, color));

        public static void ReplaceLine(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
            => publisher.Publish(LogMessage.ReplaceLine(message, level, color));
    }
}