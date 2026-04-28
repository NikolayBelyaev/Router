using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kit.ExecutableKit.HealthCheck
{
    public class DefaultHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            var result = new HealthCheckResult(HealthStatus.Healthy);
            return Task.FromResult(result);
        }
    }
}