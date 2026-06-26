using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegramBot.src.Entities.Clothing;
using telegramBot.src.Entities;
using telegramBot.src.Entities.Session;
using telegramBot.src.Services;
using telegramBot.src.UI;

namespace telegramBot.src.Handlers.Flow
{
    internal class RemoveFlowHandler : IFlowHandler
    {
        public RemoveFlowHandler(ITelegramBotClient client, IClothingService service, SessionManager sessionManager)
        {
            _client = client;
            _clothingService = service;
            _sessionManager = sessionManager;
        }

        private readonly ITelegramBotClient _client;
        private readonly IClothingService _clothingService;
        private readonly SessionManager _sessionManager;

        public SessionMode Mode => SessionMode.RemoveItem;

        public async Task HandleAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            var sessionStep = session.Step;
            switch (sessionStep)
            {
                case SessionStep.SelectType:
                    await HandleTypeStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.SelectItem:
                    await HandleItemSelectionStepAsync(session, message, cancellationToken);
                    break;
                case SessionStep.ConfirmAction:
                    await HandleConfirmationStepAsync(session, message, cancellationToken);
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

            var items = await _clothingService.GetItemByTypeAsync(message.From!.Id, type, 0, cancellationToken);

            if (items == null || items.Count == 0)
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"В категории {message.Text} у вас нет вещей. Пожалуйста, выберите другую категорию",
                    replyMarkup: Keyboards.ClothingTypes,
                    cancellationToken: cancellationToken);
                return;
            }

            session.Type = type;
            session.CurrentPage = 0;
            session.Step = SessionStep.SelectItem;
            
            await UIHelper.RenderClothingAlbumAsync(_client, message.Chat!.Id, session, items, cancellationToken);
        }

        private async Task HandleItemSelectionStepAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text) return;

            string action = message.Text!;

            if (action == "➡ Вперёд")
            {
                session.CurrentPage++;
                var items = await _clothingService.GetItemByTypeAsync(message.From!.Id, session.Type!.Value, session.CurrentPage, cancellationToken);
                await UIHelper.RenderClothingAlbumAsync(_client, message.Chat!.Id, session, items, cancellationToken);
            }
            else if (action == "⬅️ Назад" && session.CurrentPage > 0)
            {
                session.CurrentPage--;
                var items = await _clothingService.GetItemByTypeAsync(message.From!.Id, session.Type!.Value, session.CurrentPage, cancellationToken);
                await UIHelper.RenderClothingAlbumAsync(_client, message.Chat!.Id, session, items, cancellationToken);
            }

            else if (action.StartsWith("Удалить №"))
            {
                if (int.TryParse(action.Replace("Удалить №", ""), out int itemNumber))
                {
                    var items = await _clothingService.GetItemByTypeAsync(message.From!.Id, session.Type!.Value, session.CurrentPage, cancellationToken);

                    if (itemNumber > 0 && itemNumber <= items.Count)
                    {
                        var targetItem = items[itemNumber - 1];

                        session.SelectedItemId = targetItem.Id; 
                        session.ClothingName = targetItem.Name; 
                        session.Step = SessionStep.ConfirmAction;

                        await UIHelper.ClearOldMessagesAsync(_client, message.Chat.Id, session);

                        var confirmationKeyboard = new ReplyKeyboardMarkup(new[]
                        {
                            new KeyboardButton[] { "✅ Да, удалить", "❌ Отмена" }
                        })
                        { ResizeKeyboard = true };

                        await _client.SendMessage(
                            chatId: message.Chat.Id,
                            text: $"Вы уверены, что хотите окончательно удалить вещь '{targetItem.Name}' из гардероба?",
                            replyMarkup: confirmationKeyboard,
                            cancellationToken: cancellationToken);
                    }
                }
            }
        }
        private async Task HandleConfirmationStepAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text) return;

            long userId = message.From!.Id;

            if (message.Text == "✅ Да, удалить" && session.SelectedItemId.HasValue)
            {
                await _clothingService.DeleteItemAsync(session.SelectedItemId.Value, cancellationToken);

                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Вещь '{session.ClothingName}' успешно удалена!",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }
            else
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Удаление отменено.",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }

            _sessionManager.ClearSession(userId);
        }
    }
}
