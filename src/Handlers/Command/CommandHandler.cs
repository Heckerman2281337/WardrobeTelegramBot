using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using telegramBot.src.Services;
using telegramBot.src.Entities.Session;

namespace telegramBot.src.Handlers.Command
{   //Handle for commands and non-existing session
    internal class CommandHandler
    {
        public CommandHandler(ITelegramBotClient client, SessionManager sessionManager)
        {
            _client = client;
            _sessionManager = sessionManager;
        }
        private readonly ITelegramBotClient _client;
        private readonly SessionManager _sessionManager;

        public async Task HandleCommandAsync(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            if (message.Type != MessageType.Text || string.IsNullOrWhiteSpace(message.Text))
            {
                await _client.SendMessage(
                    chatId: chatId,
                    text: "Вне режима добавления вещей. Я понимаю только команды. Напиши /help, чтобы посмотреть список.",
                    cancellationToken: cancellationToken);
                return;
            }

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
                        text: "Доступные команды:\n/start - Начать работу\n/help - Помощь\n/add - Добавить вещь" +
                        "\n/remove - удалить вещь\n/outfit - подобрать образ\n/cancel - отменить операцию",
                        cancellationToken: cancellationToken);
                    break;
                case "/add":
                    _sessionManager.SetSession(message.From!.Id, SessionMode.AddItem);
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Выберите тип",
                        replyMarkup: Keyboards.ClothingTypes,
                        cancellationToken: cancellationToken);
                    break;
                case "/outfit":
                    _sessionManager.SetSession(message.From!.Id, SessionMode.CreateOutfit);
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Выберите тип",
                        replyMarkup: Keyboards.ClothingTypes,
                        cancellationToken: cancellationToken);
                    break;
                case "/remove":
                    _sessionManager.SetSession(message.From!.Id, SessionMode.RemoveItem);
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Выберите тип",
                        replyMarkup: Keyboards.ClothingTypes,
                        cancellationToken: cancellationToken);
                    break;
                default:
                    await _client.SendMessage(
                        chatId: chatId,
                        text: "Я не понимаю эту команду. Напишите /help для помощи",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
    }
}
