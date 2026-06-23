using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using telegramBot.src.Entities.Session;
using telegramBot.src.Services;

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
                case SessionStep.AddingType:
                    await HandleTypeStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.AddingPhoto:
                    await HandlePhotoStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.AddingName:
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
                    cancellationToken: cancellationToken);
                return;
            }

            session.Type = type;
            session.Step = SessionStep.AddingPhoto;

            await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Пришлите фото",
                        cancellationToken: cancellationToken);
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

            session.FileId = message.Photo![^1].FileId;
            session.Step = SessionStep.AddingName;
            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Добавьте название (Например, синие джинсы или чёрная куртка)",
                cancellationToken: cancellationToken);
            //add confirm from user
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


            try
            {
                await _clothingService.AddClothingAsync(
                session.ClothingName,
                message.From!.Id,
                session.FileId!,
                session.Type!.Value);
            }
            catch (Exception ex)
            {
                await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "К сожалению, произошла ошибка, попробуйте снова",
                cancellationToken: cancellationToken);

                Console.WriteLine(ex); //logger later
            }


            _sessionManager.ClearSession(userId);

            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: "Вещь добавлена в гардероб!",
                cancellationToken: cancellationToken);
        }
    }
}
