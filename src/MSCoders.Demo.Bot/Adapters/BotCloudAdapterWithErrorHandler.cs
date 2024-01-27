using Encamina.Enmarcha.Bot.Abstractions.Adapters;
using Encamina.Enmarcha.Bot.Adapters;

namespace MSCoders.Demo.Bot.Adapters;

internal sealed class BotCloudAdapterWithErrorHandler : BotCloudAdapterWithErrorHandlerBase
{
    public BotCloudAdapterWithErrorHandler(IBotAdapterOptions<BotCloudAdapterWithErrorHandlerBase> adapterOptions) : base(adapterOptions)
    {
        InitializeDefaultMiddlewares();
    }
}