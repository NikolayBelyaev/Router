using System;
using System.Collections.Generic;
using Kit.Logging.Abstraction;
using Microsoft.Extensions.Logging;

namespace Kit.Logging.Logger
{
    public class KitLogger : IKitLogger
    {
        private readonly ILogger _logger;

        private bool _silent;

        public KitLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(KitLogLevel kitLogLevel, string message, Exception exception)
        {
            if (_silent)
                return;
            
            var logLevel = ConvertKitLogLevelToLogLevel(kitLogLevel);
            var eventId = 0;

            _logger.Log(logLevel, eventId, message, exception, Formatter);
        }

        public void SetSilent(bool silent)
        {
            _silent = silent;
        }

        private string Formatter(object message, Exception exception)
        {
            return message.ToString();
        }

        private LogLevel ConvertKitLogLevelToLogLevel(KitLogLevel kitLogLevel)
        {
            var mapping = new Dictionary<KitLogLevel, LogLevel>()
            {
                {KitLogLevel.Debug, LogLevel.Debug},
                {KitLogLevel.Info, LogLevel.Information},
                {KitLogLevel.Warning, LogLevel.Warning},
                {KitLogLevel.Error, LogLevel.Error},
                {KitLogLevel.Critical, LogLevel.Critical},
            };

            if (!mapping.ContainsKey(kitLogLevel))
            {
                throw new ArgumentOutOfRangeException(nameof(kitLogLevel));
            }

            return mapping[kitLogLevel];
        }
    }
}