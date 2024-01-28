using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace MSCoders.Demo.Bot.Bots;

internal class Bot(BotConfiguration botConfiguration, IEnumerable<Dialog> dialogs) : ActivityHandler
{
    protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
    {
        switch (turnContext.Activity.Name)
        {
            case @"webchat/greetings":
                await dialogs.Single(d => d is Dialogs.Greetings.GreetingsDialog)
                             .RunAsync(turnContext, botConfiguration.ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                break;
        }

        await base.OnEventActivityAsync(turnContext, cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        await base.OnMessageActivityAsync(turnContext, cancellationToken);

        await dialogs.Single(d => d is Dialogs.Root.RootDialog)
                     .RunAsync(turnContext, botConfiguration.ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
    }
}
