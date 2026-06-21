using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using telegramBot.src.Entities;
using telegramBot.src.Services;

namespace telegramBot.src.Handlers
{
    internal class BotHandler
    {
        private readonly Dictionary<string, ClothingItemType> _clothingType = new Dictionary<string, ClothingItemType>
        {
            { "Вверх", ClothingItemType.Top },
            { "Низ", ClothingItemType.Bottom },
            { "Обувь", ClothingItemType.Footwear },
            { "Голова", ClothingItemType.Headwear }, 
            { "Всё тело", ClothingItemType.FullBody }
        };


        public BotHandler(ITelegramBotClient botClient, IClothingService service, SessionManager sessionManager) 
        {
            _client = botClient;
            _clothingService = service;
            _sessionManager = sessionManager;
        }

        private readonly ITelegramBotClient _client;
        private readonly IClothingService _clothingService;
        private readonly SessionManager _sessionManager;

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var userId = update.Message!.From!.Id;

            if (update.Message?.Type == MessageType.Text && update.Message.Text!.Trim().StartsWith("/cancel"))
            {
                _sessionManager.ClearSession(userId); //remove session if user write /cancel

                await _client.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "Операция отменена.",
                    cancellationToken: cancellationToken);
                return;
            }

            var session = _sessionManager.GetSession(userId);
            if (session == null)
            {
                await HandleCommandAsync(update.Message, cancellationToken);
            }
            else
            {
                var sessionStep = session.Step;
                switch (sessionStep)
                {
                    case SessionStep.AddingType:
                        await HandleTypeAddingAsync(session, update.Message, cancellationToken);
                        break;
                    case SessionStep.AddingPhoto:
                        await HandlePhotoAddingAsync(session, update.Message, cancellationToken);
                        break;
                    case SessionStep.AddingName:
                        await HandleNameAddingAsync(userId, session, update.Message, cancellationToken);
                        break;
                }
            }
        }
        //Handle for commands and non-existing session
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
                        text: "Доступные команды:\n/start - Начать работу\n/add - Добавить вещь\n/remove - удалить вещь" +
                        "\n/help - Помощь\n",
                        cancellationToken: cancellationToken);
                    break;
                case "/add":
                    _sessionManager.SetSession(message.From!.Id);
                    await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Выберите тип",
                        replyMarkup: new[]
                        {"Вверх", "Низ", "Обувь", "Голова", "Всё тело"},
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
        //Handlers for existing session
        private async Task HandleTypeAddingAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text || !_clothingType.ContainsKey(message.Text!))
            {
                await _client.SendMessage(chatId: message.Chat.Id,
                    text: "Пожалуйста, выберите тип одежды",
                    cancellationToken: cancellationToken);
                return;
            }   

            session.Type = _clothingType[message.Text!];
            session.Step = SessionStep.AddingPhoto;

            await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Пришлите фото",
                        cancellationToken: cancellationToken);
        }
        private async Task HandlePhotoAddingAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Photo)
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "На этом этапе я принимаю только фото. Пожалуйста, пришлите фото",
                    cancellationToken: cancellationToken);
                return;
            }

            session.FileId = message.Photo![^1].FileId;
            session.Step = SessionStep.AddingName;
            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Добавьте название (Например, синие джинсы или чёрная куртка)",
                cancellationToken: cancellationToken);
            //add confirm from user
        }
        private async Task HandleNameAddingAsync(long userId, UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text)
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Пожалуйста, введите название вещи",
                    cancellationToken: cancellationToken);
                return;
            }
            session.ClothingName = message.Text!;

            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Вещь добавлена в гардероб!",
                cancellationToken: cancellationToken);

            await _clothingService.AddClothingAsync(
                session.ClothingName, 
                message.Id, 
                message.From!.Id, 
                session.FileId!, 
                session.Type!.Value);

            _sessionManager.ClearSession(userId);

        }   
    }
}
