using Azure;
using Azure.AI.OpenAI;

using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;

using MSCoders.Demo.Services.VacationPlanner.Plugins;

namespace MSCoders.Demo.Services.VacationPlanner.Extensions;

internal static class IServiceCollectionExtensions
{
    internal static IServiceCollection AddAzureAIServices(this IServiceCollection services)
    {
        // Add Azure OpenAI Client
        services.AddSingleton(serviceProvider =>
        {
            var azureOpenAIOptions = serviceProvider.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            var oaiClientOptions = new OpenAIClientOptions(azureOpenAIOptions.ServiceVersion);
            oaiClientOptions.Retry.MaxRetries = 3;
            oaiClientOptions.Retry.NetworkTimeout = TimeSpan.FromMinutes(5);

            return new OpenAIClient(azureOpenAIOptions.Endpoint, new AzureKeyCredential(azureOpenAIOptions.Key), oaiClientOptions);
        });

        // Add Semantic Memory
        services.AddSingleton(serviceProvider =>
        {
            var azureOpenAIOptions = serviceProvider.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            return new MemoryBuilder()
                    .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingsModelDeploymentName, azureOpenAIOptions.Endpoint.AbsoluteUri, azureOpenAIOptions.Key, modelId: azureOpenAIOptions.EmbeddingsModelName)
                    .WithMemoryStore(new VolatileMemoryStore())
                    .Build();
        });

        // Add Semantic Kernel
        services.AddScoped(serviceProvider =>
        {
            var oaiClient = serviceProvider.GetRequiredService<OpenAIClient>();
            var azureOpenAIOptions = serviceProvider.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.Services.AddLogging(configure =>
            {
                configure.AddApplicationInsights(configureTelemetryConfiguration: (telemetryConfiguration) =>
                {
                    telemetryConfiguration.ConnectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(@"ApplicationInsights");
                }, configureApplicationInsightsLoggerOptions: (options) => { });
            });

            kernelBuilder.AddAzureOpenAIChatCompletion(azureOpenAIOptions.ChatModelDeploymentName, oaiClient, modelId: azureOpenAIOptions.ChatModelName)
                         .AddAzureOpenAITextEmbeddingGeneration(azureOpenAIOptions.EmbeddingsModelDeploymentName, oaiClient, modelId: azureOpenAIOptions.EmbeddingsModelName)
                         ;

            kernelBuilder.Services.AddDaprClient();

            kernelBuilder.Services.AddSingleton(serviceProvider.GetRequiredService<ISemanticTextMemory>());

            var kernel = kernelBuilder.Build();

            // Add plugins from this project...
            kernel.ImportPluginFromType<FlightsCatalogPlugin>();
            kernel.ImportPluginFromType<FunCatalogPlugin>();
            kernel.ImportPluginFromType<HotelsCatalogPlugin>();

            return kernel;
        });

        return services;
    }
}
