using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace MSCoders.Demo.Services.Common;

public static class IHostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
                        .AddCheck(@"self", () => HealthCheckResult.Healthy(), [@"live"]) // Add a default liveness check to ensure app is responsive
                        ;

        builder.Services.AddServiceDiscovery()
                        .ConfigureHttpClientDefaults(http =>
                        {
                            http.AddStandardResilienceHandler(); // Turn on resilience by default.
                            http.UseServiceDiscovery(); // Turn on service discovery by default.
                        })
                        ;

        return builder;
    }
}
