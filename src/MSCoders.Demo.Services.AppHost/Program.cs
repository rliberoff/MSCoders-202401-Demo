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

// Get configuration parameters to pass them as environmental variables.
var appInsightsConnectionString = builder.Configuration.GetConnectionString(@"ApplicationInsights");

var ChatModelDeploymentName = builder.Configuration[@"AzureOpenAIOptions:ChatModelDeploymentName"];
var ChatModelName = builder.Configuration[@"AzureOpenAIOptions:ChatModelName"];
var EmbeddingsModelDeploymentName = builder.Configuration[@"AzureOpenAIOptions:EmbeddingsModelDeploymentName"];
var EmbeddingsModelName = builder.Configuration[@"AzureOpenAIOptions:EmbeddingsModelName"];
var Endpoint = builder.Configuration[@"AzureOpenAIOptions:Endpoint"];
var Key = builder.Configuration[@"AzureOpenAIOptions:Key"];

var MicrosoftAppId = builder.Configuration[@"MicrosoftAppId"];
var MicrosoftAppPassword = builder.Configuration[@"MicrosoftAppPassword"];
var MicrosoftAppTenantId = builder.Configuration[@"MicrosoftAppTenantId"];
var MicrosoftAppType = builder.Configuration[@"MicrosoftAppType"];

var DirectLineEndpoint = builder.Configuration[@"DirectLineOptions:Endpoint"];
var DirectLineToken = builder.Configuration[@"DirectLineOptions:Token"];

builder.AddDapr();

var serviceFlightsCatalog = builder.AddProject<Projects.MSCoders_Demo_Services_FlightsCatalog>(@"flights-catalog")
    .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
    .WithDaprSidecar();

builder.AddProject<Projects.MSCoders_Demo_Services_FunCatalog>(@"fun-catalog")
       .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
       .WithDaprSidecar();

var serviceHotelsCatalog = builder.AddProject<Projects.MSCoders_Demo_Services_HotelsCatalog>(@"hotels-catalog")
    .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
    .WithDaprSidecar();

builder.AddProject<Projects.MSCoders_Demo_Bot>(@"bot-backend")
       .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
       .WithEnvironment(@"MicrosoftAppId", MicrosoftAppId)
       .WithEnvironment(@"MicrosoftAppPassword", MicrosoftAppPassword)
       .WithEnvironment(@"MicrosoftAppTenantId", MicrosoftAppTenantId)
       .WithEnvironment(@"MicrosoftAppType", MicrosoftAppType)
       .WithDaprSidecar();

builder.AddProject<Projects.MSCoders_Demo_Services_VacationPlanner>(@"vacation-planner")
       .WithEnvironment(@"ConnectionStrings:ApplicationInsights", appInsightsConnectionString)
       .WithEnvironment(@"AzureOpenAIOptions:ChatModelDeploymentName", ChatModelDeploymentName)
       .WithEnvironment(@"AzureOpenAIOptions:ChatModelName", ChatModelName)
       .WithEnvironment(@"AzureOpenAIOptions:EmbeddingsModelDeploymentName", EmbeddingsModelDeploymentName)
       .WithEnvironment(@"AzureOpenAIOptions:EmbeddingsModelName", EmbeddingsModelName)
       .WithEnvironment(@"AzureOpenAIOptions:Endpoint", Endpoint)
       .WithEnvironment(@"AzureOpenAIOptions:Key", Key)
       .WithReference(serviceFlightsCatalog)
       .WithReference(serviceHotelsCatalog)
       .WithDaprSidecar()
       ;

builder.AddNpmApp("bot-frontend", "../MSCoders.Demo.WebChat", "start")
       .WithEnvironment(@"DIRECT_LINE_DOMAIN", DirectLineEndpoint)
       .WithEnvironment(@"DIRECT_LINE_TOKEN", DirectLineToken)
       ;

builder.Build().Run();
