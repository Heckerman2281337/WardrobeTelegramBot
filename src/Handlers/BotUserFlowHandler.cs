using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using telegramBot.src.Entities;
using telegramBot.src.Services;
using Telegram.Bot;

namespace telegramBot.src.Handlers
{   //Handler for existing session
    internal class BotUserFlowHandler
    {
        private readonly Dictionary<string, ClothingItemType> _clothingType = new Dictionary<string, ClothingItemType>
        {
            { "Вверх", ClothingItemType.Top },
            { "Низ", ClothingItemType.Bottom },
            { "Обувь", ClothingItemType.Footwear },
            { "Голова", ClothingItemType.Headwear },
            { "Всё тело", ClothingItemType.FullBody }
        };

        public BotUserFlowHandler(ITelegramBotClient client, IClothingService clothingService, SessionManager sessionManager) 
        { 
            _client = client;
            _clothingService = clothingService;
            _sessionManager = sessionManager;
        }
        private readonly ITelegramBotClient _client;
        private readonly IClothingService _clothingService;
        private readonly SessionManager _sessionManager;

        public async Task HandleFlowAsync(Message message, long userId, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(userId);
            if (session == null) return;

            var sessionStep = session.Step;
            switch (sessionStep)
            {
                case SessionStep.AddingType:
                    await HandleTypeAddingAsync(session, message, cancellationToken);
                    break;
                case SessionStep.AddingPhoto:
                    await HandlePhotoAddingAsync(session, message, cancellationToken);
                    break;
                case SessionStep.AddingName:
                    await HandleNameAddingAsync(userId, session, message, cancellationToken);
                    break;
            }
        }

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

            _sessionManager.ClearSession(userId);

            await _clothingService.AddClothingAsync(
                session.ClothingName,
                message.From!.Id,
                session.FileId!,
                session.Type!.Value);
        }
    }
}
