using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

static class BotHandlers
{
    private static readonly ConcurrentDictionary<long, UserSession> _sessions = new();
    private static string telegramToken;
    public static void Initialize(string token)
    {
        telegramToken = token;
    }

    // method that works with TgBot check and scan photo, 
    // then use MindeeService method for parse and send you respond
    public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Message is not { } message)
            return;

        long chatId = message.Chat.Id;
        var session = _sessions.GetOrAdd(chatId, _ => new UserSession());

        if (message.Type == MessageType.Photo)
        {
            var fileId = message.Photo.Last().FileId;
            var file = await bot.GetFileAsync(fileId, cancellationToken: token);
            // token uses already in variable for safety
            var fileUrl = $"https://api.telegram.org/file/bot{telegramToken}/{file.FilePath}";
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

            try
            {
                using var httpClient = new HttpClient();
                var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                await System.IO.File.WriteAllBytesAsync(tempPath, fileBytes);

                if (session.Step == UserStep.AwaitingPassportPhoto)
                {
                    var result = await MindeeService.ParseDocument(tempPath, "Passport");
                    session.PassportData = result;
                    session.Step = UserStep.ConfirmPassportData;

                    await bot.SendTextMessageAsync(chatId, $"Вилучені дані паспорта:\n{result}\n\nПідтвердіть, будь ласка, чи вірні дані? (Так/Ні)");
                }
                else if (session.Step == UserStep.AwaitingRegistrationPhoto)
                {
                    var result = await MindeeService.ParseDocument(tempPath, "RegistrationCertificate");
                    session.RegistrationData = result;
                    session.Step = UserStep.ConfirmRegistrationData;

                    await bot.SendTextMessageAsync(chatId, $"Вилучені дані техпаспорта:\n{result}\n\nПідтвердіть, будь ласка, чи вірні дані? (Так/Ні)");
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, "Очікую підтвердження або інструкції. Будь ласка, відповідайте текстом.");
                }
            }
            catch (Exception ex)
            {
                await bot.SendTextMessageAsync(chatId, $"❌ Помилка при обробці документа: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }
        else if (message.Type == MessageType.Text)
        {
            var text = message.Text?.Trim().ToLower();

            switch (session.Step)
            {
                case UserStep.ConfirmPassportData:
                    if (text == "так")
                    {
                        session.Step = UserStep.AwaitingRegistrationPhoto;
                        await bot.SendTextMessageAsync(chatId, "Дякую! Тепер надішліть фото техпаспорта.");
                    }
                    else if (text == "ні")
                    {
                        session.PassportData = "";
                        session.Step = UserStep.AwaitingPassportPhoto;
                        await bot.SendTextMessageAsync(chatId, "Будь ласка, надішліть фото паспорта ще раз.");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(chatId, "Будь ласка, відповідайте 'Так' або 'Ні'.");
                    }
                    break;

                case UserStep.ConfirmRegistrationData:
                    if (text == "так")
                    {
                        session.Step = UserStep.ConfirmPrice;
                        await bot.SendTextMessageAsync(chatId, "Фіксована ціна страховки становить 100 доларів США. Ви згодні з ціною? (Так/Ні)");
                    }
                    else if (text == "ні")
                    {
                        session.RegistrationData = "";
                        session.Step = UserStep.AwaitingRegistrationPhoto;
                        await bot.SendTextMessageAsync(chatId, "Будь ласка, надішліть фото техпаспорта ще раз.");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(chatId, "Будь ласка, відповідайте 'Так' або 'Ні'.");
                    }
                    break;

                case UserStep.ConfirmPrice:
                    if (text == "так")
                    {
                        session.Step = UserStep.Completed;
                        string policy = InsurancePolicy.GenerateInsurancePolicy(session);
                        await bot.SendTextMessageAsync(chatId, $" Ваш страховий поліс:\n{policy}");
                        _sessions.TryRemove(chatId, out _);
                    }
                    else if (text == "ні")
                    {
                        await bot.SendTextMessageAsync(chatId, "Вибачте, 100 доларів США — єдина доступна ціна.");
                        _sessions.TryRemove(chatId, out _);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(chatId, "Будь ласка, відповідайте 'Так' або 'Ні'.");
                    }
                    break;

                default:
                    await bot.SendTextMessageAsync(chatId, "Будь ласка, надішліть фото паспорта для початку.");
                    session.Step = UserStep.AwaitingPassportPhoto;
                    break;
            }
        }
    }


    public static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Помилка: {exception.Message}");
        return Task.CompletedTask;
    }
}
