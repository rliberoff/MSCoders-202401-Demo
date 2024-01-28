using Dapr.Client;

using Encamina.Enmarcha.DependencyInjection;

using Microsoft.Bot.Builder;

namespace MSCoders.Demo.Bot.Dialogs.Root;

/// <summary>
/// Represents the configuration for the root dialog, defining dependencies.
/// </summary>
/// <param name="BotTelemetryClient">The <see cref="IBotTelemetryClient"/> from this configuration</param>
/// <param name="DaprClient">The <see cref="DaprClient"/> from this configuration</param>
[AutoRegisterService(ServiceLifetime.Singleton)]
internal sealed record RootDialogConfiguration(IBotTelemetryClient BotTelemetryClient, DaprClient DaprClient);
