using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace Frinfo.API.HealthChecks
{
   public class BasicHealthCheck : IHealthCheck
   {
      public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
      {
         return Task.FromResult(HealthCheckResult.Healthy("OK"));
      }
   }
}
