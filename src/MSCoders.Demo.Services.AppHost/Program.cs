using System.Diagnostics;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

/* Load Configuration */

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

var appInsightsConnectionString = builder.Configuration.GetConnectionString(@"ApplicationInsights");

builder.AddDapr();

////var serviceHistoricalWeatherLookup = builder.AddProject<Projects.MSCoders_Demo_Services_HistoricalWeatherLookup>(@"historical-weather-lookup")
////    .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
////    .WithDaprSidecar()
////    ;

var serviceFlightsCatalog = builder.AddProject<Projects.MSCoders_Demo_Services_FlightsCatalog>(@"flights-catalog")
    .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
    .WithDaprSidecar();

builder.AddProject<Projects.MSCoders_Demo_Services_FunCatalog>(@"fun-catalog")
       .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
       .WithDaprSidecar();

var serviceHotelsCatalog = builder.AddProject<Projects.MSCoders_Demo_Services_HotelsCatalog>(@"hotels-catalog")
    .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
    .WithDaprSidecar();

////builder.AddProject<Projects.MSCoders_Demo_Bot>(@"mscoders.demo.bot");

builder.AddProject<Projects.MSCoders_Demo_Services_VacationPlanner>(@"vacation-planner")
       .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
       //.WithReference(serviceHistoricalWeatherLookup)
       .WithReference(serviceFlightsCatalog)
       .WithReference(serviceHotelsCatalog)
       .WithDaprSidecar()
       ;



builder.Build().Run();
