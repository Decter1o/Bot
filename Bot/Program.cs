using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;

namespace Bot
{
    internal class Program
    {
        static async Task Main()
        {
            IConfiguration config = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                        .Build();

            string token = config["TelegramBot:token"];
            var botClient = new TelegramBotClient(token);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // получаем все типы обновлений
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync(cts.Token);
            Console.WriteLine($"Бот @{me.Username} запущен.");

            Console.ReadLine();
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message && message.Text is { } text)
            {
                Console.WriteLine($"Получено сообщение: {text} от {message.Chat.Id}");
                await bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Привет! Ты написал: " + text
                );
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
