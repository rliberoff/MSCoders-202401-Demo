using Encamina.Enmarcha.Bot.Abstractions.Dialogs;

using Microsoft.ApplicationInsights.AspNetCore;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace MSCoders.Demo.Bot.Bots;

/// <summary>
/// Initializes a new instance of the <see cref="Bot{TRootDialog}"/> class.
/// </summary>
/// <param name="rootDialog">Associated root (or main) dialog of this bot.</param>
/// <param name="botConfiguration">Configuration elements and parameters for this bot.</param>
/// <param name="logger"></param>
internal class Bot<TRootDialog>(TRootDialog rootDialog, BotConfiguration botConfiguration, ILogger<Bot<TRootDialog>> logger) 
    : BotDialogActivityHandlerBase<TRootDialog>(rootDialog, botConfiguration.ConversationState, botConfiguration.UserState, logger) where TRootDialog : Dialog
{

    /// <inheritdoc/>
    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        await base.OnMessageActivityAsync(turnContext, cancellationToken);

        await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
    }
}
