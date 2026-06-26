using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using telegramBot.src.Entities.Session;
using telegramBot.src.Services;
using Telegram.Bot.Types.ReplyMarkups;
using telegramBot.src.UI;

namespace telegramBot.src.Handlers.Flow
{
    internal class AddFlowHandler : IFlowHandler
    {
        public AddFlowHandler(ITelegramBotClient client, IClothingService service, SessionManager sessionManager)
        {
            _client = client;
            _clothingService = service;
            _sessionManager = sessionManager;
        }

        private readonly ITelegramBotClient _client;
        private readonly IClothingService _clothingService;
        private readonly SessionManager _sessionManager;

        public SessionMode Mode => SessionMode.AddItem;
        public async Task HandleAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            var sessionStep = session.Step;
            switch (sessionStep)
            {
                case SessionStep.SelectType:
                    await HandleTypeStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.SelectPhoto:
                    await HandlePhotoStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.SelectName:
                    await HandleNameStepAsync(message.From!.Id, session, message, cancellationToken);
                    break;
            }
        }

        private async Task HandleTypeStepAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text || !ClothingMap.Types.TryGetValue(message.Text!, out var type))
            {
                await _client.SendMessage(chatId: message.Chat.Id,
                    text: "Пожалуйста, выберите тип одежды",
                    replyMarkup: Keyboards.ClothingTypes,
                    cancellationToken: cancellationToken);
                return;
            }

            session.Type = type;
            session.Step = SessionStep.SelectPhoto;
            var botMessage = await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Пришлите фото",
                        replyMarkup: new ReplyKeyboardRemove(), 
                        cancellationToken: cancellationToken);

            session.MessagesToDelete.Add(botMessage.MessageId);
        }
        private async Task HandlePhotoStepAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Photo)
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "На этом этапе я принимаю только фото. Пожалуйста, пришлите фото",
                    cancellationToken: cancellationToken);
                return;
            }
            session.MessagesToDelete.Add(message.MessageId);

            session.FileId = message.Photo![^1].FileId;
            session.Step = SessionStep.SelectName;

            var botMessage = await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Добавьте название (Например, синие джинсы или чёрная куртка)",
                cancellationToken: cancellationToken);

            session.MessagesToDelete.Add(botMessage.MessageId);
        }
        private async Task HandleNameStepAsync(long userId, UserSession session, Message message, CancellationToken cancellationToken)
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

            session.MessagesToDelete.Add(message.MessageId);
            
            try
            {
                await _clothingService.AddClothingAsync(
                session.ClothingName,
                message.From!.Id,
                session.FileId!,
                session.Type!.Value,
                cancellationToken);
            }
            catch (Exception ex)
            {
                await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "К сожалению, произошла ошибка, попробуйте снова",
                cancellationToken: cancellationToken);

                Console.WriteLine(ex); //logger later   
                return;
            }

            await UIHelper.ClearOldMessagesAsync(_client, message.Chat.Id, session);

            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Вещь добавлена в гардероб!",
                cancellationToken: cancellationToken);

            _sessionManager.ClearSession(userId);
        }
    }
}
