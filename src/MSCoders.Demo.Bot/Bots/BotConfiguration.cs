using Microsoft.Bot.Builder;

using Encamina.Enmarcha.DependencyInjection;

namespace MSCoders.Demo.Bot.Bots;

/// <summary>
/// Configuration of a bot's activity handler.
/// </summary>
[AutoRegisterService(ServiceLifetime.Singleton)]
internal class BotConfiguration(ConversationState conversationState, UserState userState)
{
    public ConversationState ConversationState { get; init; } = conversationState;

    public UserState UserState { get; init; } = userState;
}
