using Telegram.Bot;
using Telegram.Bot.Types;

namespace telegramBot.src
{
    internal class BotHandler
    {
        public BotHandler(ITelegramBotClient botClient) 
        {
            _client = botClient;
        }

        private readonly ITelegramBotClient _client;
        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Photo is not null)
            {
                await HandlePhotoAsync(message, cancellationToken);
                return;
            }

            if (message.Text is { } messageText)
            {
                await HandleCommandAsync(message, cancellationToken);
            }
        }

        private async Task HandlePhotoAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var photoId = message.Photo!.Last().FileId;

            await _client.SendMessage(chatId, "Фото успешно добавлено в ваш гардероб!", cancellationToken: cancellationToken);
            return;
        }

        private async Task HandleCommandAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var command = message.Text!.Trim().Split(' ')[0];

            switch (command)
            {
                case "/start":
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Привет! Я бот гардероб. Я принимаю только фото. Напиши /help для помощи.",
                        cancellationToken: cancellationToken);
                    break;
                case "/help":
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Доступные команды:\n/start - Начать работу\n/help - Помощь",
                        cancellationToken: cancellationToken);
                    break;
                default:
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Я не понимаю эту команду.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
    }
}
