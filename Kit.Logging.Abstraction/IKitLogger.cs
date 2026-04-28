using System;

namespace Kit.Logging.Abstraction
{
    public interface IKitLogger
    {
        void Log(KitLogLevel kitLogLevel, string message, Exception exception);
    }

    public static class KitLoggerExtensions
    {
        public static void Verbose(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Debug, message, null);
        }

        public static void Debug(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Debug, message, null);
        }

        public static void Info(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Info, message, null);
        }

        public static void Warning(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Warning, message, null);
        }

        public static void Warning(this IKitLogger logger, string message, Exception exception)
        {
            logger.Log(KitLogLevel.Warning, message, exception);
        }

        public static void Error(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Error, message, null);
        }

        public static void Error(this IKitLogger logger, Exception exception)
        {
            logger.Log(KitLogLevel.Error, exception.Message, exception);
        }

        public static void Error(this IKitLogger logger, string message, Exception exception)
        {
            logger.Log(KitLogLevel.Error, message, exception);
        }

        public static void Critical(this IKitLogger logger, string message)
        {
            logger.Log(KitLogLevel.Critical, message, null);
        }

        public static void Critical(this IKitLogger logger, Exception exception)
        {
            logger.Log(KitLogLevel.Critical, exception.Message, exception);
        }

        public static void Critical(this IKitLogger logger, string message, Exception exception)
        {
            logger.Log(KitLogLevel.Critical, message, exception);
        }
    }
}