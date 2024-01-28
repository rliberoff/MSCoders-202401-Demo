using System.Reflection;

using Encamina.Enmarcha.DependencyInjection;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace MSCoders.Demo.Bot.Dialogs.Greetings;

[AutoRegisterService(ServiceLifetime.Singleton, typeof(Dialog))]
internal sealed class GreetingsDialog : Dialog
{
    private readonly IMessageActivity greetingsMessageActivity;

    public GreetingsDialog()
    {
        greetingsMessageActivity = MessageFactory.Attachment(new HeroCard()
        {
            Images = new List<CardImage>()
            {
                new CardImage()
                {
                    Url = LoadGreetingsImageFromAssemblyEmbeddedResource(),
                },
            },
            Title = @"¡Bienvenidos!",
            Subtitle = @"Mas allá del patrón RAG: Creado aplicaciones inteligentes con Semantic Kernel...",
            Text = @"Desde MSCoders os queremos mostrar como usando Semantic Kernel, Plugins de IA y los Planners podemos conseguir responder a cuestiones complejas que nos pidan los usuarios a partir de varias fuentes de datos y el patrón RAG (Retrieval-Augmented Generation)",
        }.ToAttachment());
    }

    /// <inheritdoc/>
    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
    {
        var result = await dc.Context.SendActivityAsync(greetingsMessageActivity, cancellationToken);

        return await dc.EndDialogAsync(result, cancellationToken);
    }

    private static string LoadGreetingsImageFromAssemblyEmbeddedResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(@"MSCoders.Demo.Bot.Resources.MSCoders.png");
        var count = (int)stream.Length;
        var data = new byte[count];
        stream.Read(data, 0, count);
        return $@"data:image/png;base64,{Convert.ToBase64String(data)}";
    }
}
