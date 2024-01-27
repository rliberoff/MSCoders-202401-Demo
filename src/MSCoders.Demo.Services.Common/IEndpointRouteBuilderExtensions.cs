using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace MSCoders.Demo.Services.Common;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapDefaultEndpoints(this WebApplication app)
    {
        // All health checks must pass for app to be considered ready to accept traffic after starting
        app.MapHealthChecks(@"/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks(@"/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains(@"live"),
        });

        return app;
    }
}
