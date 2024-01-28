using System.Text.Json;
using System.Text.RegularExpressions;

using Encamina.Enmarcha.Core;
using Encamina.Enmarcha.DependencyInjection;

using Microsoft.Bot.Builder.Dialogs;

namespace MSCoders.Demo.Bot.Dialogs.Root;

/// <summary>
/// Root dialog managing other dialogs within the bot.
/// </summary>
[AutoRegisterService(ServiceLifetime.Singleton, typeof(Dialog))]
internal sealed class RootDialog : Dialog
{
    private readonly RootDialogConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootDialog"/> class.
    /// </summary>
    /// <param name="configuration">Configuration for the root dialog.</param>
    public RootDialog(RootDialogConfiguration configuration)
    {
        this.configuration = configuration;

        TelemetryClient = configuration.BotTelemetryClient;
    }

    /// <summary>
    /// Begins the execution of the root dialog when it is started.
    /// This method redirects to other dialogs.
    /// </summary>
    /// <param name="dc">The <see cref="DialogContext"/> for the current turn of conversation.</param>
    /// <param name="options">Optional, initial information to pass to the dialog.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
    {
        var turnContext = dc.Context;
        var ask = turnContext.Activity.Text;

        // If the user is not asking anything, then end the dialog.
        if (string.IsNullOrWhiteSpace(ask))
        {
            return await dc.EndDialogAsync(cancellationToken: cancellationToken);
        }

        using var httpRequest = configuration.DaprClient.CreateInvokeMethodRequest(HttpMethod.Post, @"vacation-planner", @"vacations/plan", new
        {
            Ask = ask,
        });

        using var httpResponse = await configuration.DaprClient.InvokeMethodWithResponseAsync(httpRequest, cancellationToken);

        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonUtils.DeserializeAnonymousType(response, new { Answer = @"" }, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        await turnContext.SendActivityAsync(Regex.Unescape(result.Answer), cancellationToken: cancellationToken);

        return await dc.EndDialogAsync(cancellationToken: cancellationToken);
    }
}
