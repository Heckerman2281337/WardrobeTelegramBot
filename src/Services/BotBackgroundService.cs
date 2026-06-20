using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegramBot.src
{
    //Background service for bot so it can shut down safely. Also its much cleaner than making everything in program.cs
    public class BotBackgroundService : BackgroundService
    {
        public BotBackgroundService(ITelegramBotClient client, IServiceProvider serviceProvider) 
        { 
            _client = client;
            _serviceProvider = serviceProvider;
        }

        private readonly ITelegramBotClient _client;
        private readonly IServiceProvider _serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var me = await _client.GetMe(stoppingToken);
            Console.WriteLine($"@{me.Username} is running... Press Ctrl+C to terminate");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // So telegram can send to bot everything no matter what type of
                // message is.
            };
            //Polling
            _client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
                );

            await Task.Delay(Timeout.Infinite, stoppingToken); // Bot is polling non-stop unless its shuts down
        }

        
        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope(); // 1 request - 1 scope
            var handler = scope.ServiceProvider.GetRequiredService<BotHandler>();

            try
            {
                await handler.HandleUpdateAsync(update, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Ошибка при обработке обновления: {ex.Message}");
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[Telegram API Error]: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}

