using Kit.Logging.Abstraction;

namespace Router.Executable.Server.Logging;

public static class PlatformLoggerExtensions
{
    public static void LogSucceededRequest(
        this IKitLogger logger, 
        string route, 
        TimeSpan timeSpan
    )
    {
        logger.Info($"Request {route.ToLower()} handled. ResponseTime: {timeSpan.TotalMilliseconds}");
    }
}