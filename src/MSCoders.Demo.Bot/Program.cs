using System.Diagnostics;

using Encamina.Enmarcha.Bot.Abstractions.Adapters;
using Encamina.Enmarcha.Bot.Adapters;

using Microsoft.ApplicationInsights.Extensibility;

using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

using Microsoft.Extensions.Logging.ApplicationInsights;

using MSCoders.Demo.Bot.Adapters;
using MSCoders.Demo.Bot.Dialogs.Root;
using MSCoders.Demo.Services.Common;
using MSCoders.Demo.Bot.Bots;

/*
 *  Load Configuration
 */

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     ;

var isDevelopment = builder.Environment.IsDevelopment();

/*
 *  Logging Configuration
 */

if (isDevelopment)
{
    builder.Logging.AddConsole();

    if (Debugger.IsAttached)
    {
        builder.Logging.AddDebug();
    }
}

var applicationInsightsConnectionString = builder.Configuration.GetConnectionString(@"ApplicationInsights");

builder.Logging.AddApplicationInsights((telemetryConfiguration) => telemetryConfiguration.ConnectionString = applicationInsightsConnectionString, (_) => { })
               .AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Trace)
               ;

/*
 *  Application Services
 */

builder.AddServiceDefaults();

builder.Services.AddDaprClient();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration)
                .AddLogging(loggingBuilder => loggingBuilder.AddApplicationInsights())
                .AddAutoRegisterServicesFromAssembly<Program>()
                .AddRouting()
                .AddHealthChecks()
                ;

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = true;
})
;

// Bot configuration...
builder.Services.AddSingleton<IBotTelemetryClient, BotTelemetryClient>()
                .AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>()
                .AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>()
                .AddSingleton(sp =>
                {
                    var telemetryClient = sp.GetService<IBotTelemetryClient>();
                    return new TelemetryLoggerMiddleware(telemetryClient, logPersonalInformation: true);
                })
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, TelemetryLoggerMiddleware>(sp =>
                {
                    return sp.GetRequiredService<TelemetryLoggerMiddleware>();
                })
                .AddSingleton(sp =>
                {
                    var loggerMiddleware = sp.GetRequiredService<TelemetryLoggerMiddleware>();
                    var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                    return new TelemetryInitializerMiddleware(contextAccessor, loggerMiddleware, logActivityTelemetry: true);
                })
                .AddSingleton<Microsoft.Bot.Builder.IMiddleware, TelemetryInitializerMiddleware>(sp =>
                {
                    return sp.GetRequiredService<TelemetryInitializerMiddleware>();
                })
                .AddBotDefaultApplicationInsightsTelemetry(applicationInsightsConnectionString)
                .AddBotStorageInMemory()
                .AddBotDefaultStates()
                ;

// Add bot middlewares...
builder.Services.AddBotAutoSaveStateMiddleware()
                .AddBotShowTypingMiddleware()
                .AddBotConversationStateLoggerMiddleware() // This middleware records in the bot's conversation state the current conversation.
                ;

// Add bot services...
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>() // Create the Bot Framework Authentication to be used with the Bot Adapter.
                .AddSingleton<IBotFrameworkHttpAdapter, BotCloudAdapterWithErrorHandler>() // Create the Bot Adapter with error handling enabled.
                .AddSingleton<IBotAdapterOptions<BotCloudAdapterWithErrorHandlerBase>, BotCloudAdapterWithErrorHandlerServices>()
                .AddScoped<IBot, Bot>()
                ;

/*
 *  Application Middleware Configuration
 */

var app = builder.Build();

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting()
   .UseAuthentication()
   .UseAuthorization()
   .UseEndpoints(endpoints =>
   {
       endpoints.MapControllers();
   })
   ;

app.Run();
