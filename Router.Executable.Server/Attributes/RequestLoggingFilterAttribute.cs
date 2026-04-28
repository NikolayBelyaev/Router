using System.Diagnostics;
using Kit.Logging.Abstraction;
using Microsoft.AspNetCore.Mvc.Filters;
using Router.Executable.Server.Logging;

namespace Router.Executable.Server.Attributes;

public class RequestLoggingFilterAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var executedContext = await next();
        stopwatch.Stop();

        var logger = executedContext.HttpContext.RequestServices.GetService<IKitLogger>();

        var route = executedContext.HttpContext.Request.Path.Value?.ToLower();
        
        logger.LogSucceededRequest(route, stopwatch.Elapsed);
    }
}