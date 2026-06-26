using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegramBot.src.Entities.Session;
using telegramBot.src.Services;
using telegramBot.src.UI;

namespace telegramBot.src.Handlers.Flow
{
    internal class OutfitFlowHandler : IFlowHandler
    {
        public OutfitFlowHandler(ITelegramBotClient client, IClothingService service, SessionManager sessionManager)
        {
            _client = client;
            _clothingService = service;
            _sessionManager = sessionManager;
        }

        private readonly ITelegramBotClient _client;
        private readonly IClothingService _clothingService;
        private readonly SessionManager _sessionManager;

        public SessionMode Mode => SessionMode.CreateOutfit;

        public async Task HandleAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {

            session.OutfitDraft ??= new Entities.OutfitDraft();
            //loop for outfit picking
            if (message.Type == MessageType.Text)
            {
                if (message.Text == "Добавить ещё вещь")
                {
                    session.Step = SessionStep.SelectType;
                    await _client.SendMessage(message.Chat.Id, "Выберите следующий тип одежды для вашего образа:", replyMarkup: Keyboards.ClothingTypes, cancellationToken: cancellationToken);
                    return;
                }

                if (message.Text == "Образ готов!")
                {
                    await FinalizeOutfitAsync(session, message.Chat.Id, message.From!.Id, cancellationToken);
                    return;
                }
            }

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
                await _client.SendMessage(
                    chatId: message.Chat.Id,
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

            else if (action.StartsWith("Выбрать №"))
            {
                if (int.TryParse(action.Replace("Выбрать №", ""), out int itemNumber))
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
                            new KeyboardButton[] { "✅ Выбрать", "❌ Отмена" }
                        })
                        { ResizeKeyboard = true };

                        await _client.SendMessage(
                            chatId: message.Chat.Id,
                            text: $"Вы уверены, что хотите выбрать вещь '{targetItem.Name}' из гардероба?",
                            replyMarkup: confirmationKeyboard,
                            cancellationToken: cancellationToken);
                    }
                }
            }
        }

        private async Task HandleConfirmationStepAsync(UserSession session, Message message, CancellationToken cancellationToken)
        {
            if (message.Type != MessageType.Text) return;

            if (message.Text == "✅ Выбрать" && session.SelectedItemId.HasValue)
            {
                var item = await _clothingService.GetItemAsync(session.SelectedItemId.Value, cancellationToken);
                if (item != null)
                {
                    session.OutfitDraft!.Items.Add(item);
                }

                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Вещь '{session.ClothingName}' добавлена в текущий черновик образа!",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }

            else
            {
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Выбор отменён.",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }


            var continuationKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { " Добавить ещё вещь", "Образ готов!" }
            })
            { ResizeKeyboard = true };

            await _client.SendMessage(
                chatId: message.Chat.Id,
                text: $"В вашем образе сейчас предметов: {session.OutfitDraft!.Items.Count}. Что делаем дальше?",
                replyMarkup: continuationKeyboard,
                cancellationToken: cancellationToken);
        }

        private async Task FinalizeOutfitAsync(UserSession session, long chatId, long userId, CancellationToken ct)
        {
            if (session.OutfitDraft == null || session.OutfitDraft.Items.Count == 0)
            {
                await _client.SendMessage(chatId, "Вы не выбрали ни одной вещи для создания образа.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
                _sessionManager.ClearSession(userId);
                return;
            }

            await _client.SendMessage(chatId, "Собираю ваш готовый образ...", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);

            var mediaGroup = session.OutfitDraft.Items.Select((item, index) => new InputMediaPhoto(InputFile.FromFileId(item.FileId))
            {
                Caption = $"Элемент №{index + 1}: {item.Name} ({item.ClothingType})"
            }).ToArray();

            await _client.SendMediaGroup(chatId, mediaGroup, cancellationToken: ct);

            await _client.SendMessage(chatId, "Ваш новый образ скомбинирован!", cancellationToken: ct);

            _sessionManager.ClearSession(userId);
        }
    }
}
