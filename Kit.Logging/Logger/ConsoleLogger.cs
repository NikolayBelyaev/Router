using System;
using Kit.Logging.Abstraction;

namespace Kit.Logging.Logger
{
    public class ConsoleLogger : IKitLogger
    {
        public void Log(KitLogLevel kitLogLevel, string message, Exception exception)
        {
            Console.WriteLine(message);
        }
    }
}