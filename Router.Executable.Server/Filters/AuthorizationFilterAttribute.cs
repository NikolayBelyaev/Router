using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Router.Executable.Server.Options;

namespace Router.Executable.Server.Filters
{
    public class AuthorizationFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var config = context.HttpContext.RequestServices.GetRequiredService<IOptions<RouterConfiguration>>();
                
                var tokenExists = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerToken);
                var valid = tokenExists && string.Equals(config.Value.Token, headerToken);

                if(!valid)
                    throw new Exception("Router authorization failed");
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private static Claim GetClaim(IEnumerable<Claim> claims, string claimName)
        {
            var claim = claims.FirstOrDefault(x => x.Type == claimName);

            if (claim == null)
            {
                throw new Exception($"{claimName} is null");
            }

            return claim;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}