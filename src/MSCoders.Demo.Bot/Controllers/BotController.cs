using Encamina.Enmarcha.Bot.Controllers;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace MSCoders.Demo.Bot.Controllers;

public sealed class BotController : BotBaseController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotController"/> class.
    /// </summary>
    /// <param name="adapter">A bot adapter to use.</param>
    /// <param name="bot">A bot that can operate on incoming activities.</param>
    public BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : base(adapter, bot)
    {
    }
}