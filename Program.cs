using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Threading;
using v3;

class Program
{
    private static TelegramBotClient? _botClient;

    static async Task Main()
    {
        string telegramToken = "8145400716:AAH3xDNm97a0-YQ4Id4zr16CiWT-kwN5g_k";

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

        Console.WriteLine("🤖 Bot started...");
        await Task.Delay(-1);
    }
}
