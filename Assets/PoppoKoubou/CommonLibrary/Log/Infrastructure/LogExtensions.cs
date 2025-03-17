using System;
using MessagePipe;
using PoppoKoubou.CommonLibrary.Log.Domain;

namespace PoppoKoubou.CommonLibrary.Log.Infrastructure
{
    public static class LogExtensions
    {
        [Obsolete] public static void AddLine(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        [Obsolete] public static void ReplaceLine(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void Verbose(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Verbose, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void RVerbose(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Verbose, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void Log(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void RLog(this IPublisher<LogMessage> publisher, string message, LogLevel level = LogLevel.Info, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, level, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void Debug(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, LogLevel.Debug, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void RDebug(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, LogLevel.Debug, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void Info(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, LogLevel.Info, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void RInfo(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, LogLevel.Info, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void Warning(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, LogLevel.Warning, color));
            else
                UnityEngine.Debug.LogWarning(message);
        }
        public static void RWarning(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, LogLevel.Warning, color));
            else
                UnityEngine.Debug.LogWarning(message);
        }
        public static void Error(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, LogLevel.Error, color));
            else
                UnityEngine.Debug.LogError(message);
        }
        public static void RError(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, LogLevel.Error, color));
            else
                UnityEngine.Debug.LogError(message);
        }
        public static void Fatal(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.AddLine(message, LogLevel.Fatal, color));
            else
                UnityEngine.Debug.Log(message);
        }
        public static void RFatal(this IPublisher<LogMessage> publisher, string message, string color = null)
        {
            if (publisher != null)
                publisher.Publish(LogMessage.ReplaceLine(message, LogLevel.Fatal, color));
            else
                UnityEngine.Debug.Log(message);
        }
    }
}