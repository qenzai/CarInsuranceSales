using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Threading;

class Program
{
    private static TelegramBotClient? _botClient;
    private static string telegramToken = "";


    static async Task Main()
    {

        _botClient = new TelegramBotClient(telegramToken);

        using var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            BotHandlers.HandleUpdateAsync,
            BotHandlers.HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );
        BotHandlers.Initialize(telegramToken);
        Console.WriteLine("Bot started...");
        await Task.Delay(-1);
    }
}
