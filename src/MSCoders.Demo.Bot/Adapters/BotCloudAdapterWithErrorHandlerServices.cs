using Encamina.Enmarcha.Bot.Adapters;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace MSCoders.Demo.Bot.Adapters;

/// <summary>
/// A custom implementation of the options for a <see cref="BotAdapterOptionsBase{BotCloudAdapterWithErrorHandler}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BotCloudAdapterWithErrorHandlerServices"/> class.
/// </remarks>
/// <param name="botFrameworkAuthentication">An optional environment (usually, a cloud environment) used to authenticate Bot Framework Protocol network calls.</param>
/// <param name="botTelemetryClient">An optional bot telemetry client.</param>
/// <param name="botStates">An optional collection of bot states.</param>
/// <param name="botMiddlewares">An optional collection of bot middlewares.</param>
/// <param name="logger">An optional logger for the adapter.</param>
internal sealed class BotCloudAdapterWithErrorHandlerServices(
    BotFrameworkAuthentication botFrameworkAuthentication, 
    IBotTelemetryClient botTelemetryClient, 
    IEnumerable<BotState> botStates, 
    IEnumerable<IMiddleware> botMiddlewares, 
    ILogger<BotCloudAdapterWithErrorHandler> logger) : BotAdapterOptionsBase<BotCloudAdapterWithErrorHandler>(botFrameworkAuthentication, botTelemetryClient, botStates, botMiddlewares, logger)
{
}
