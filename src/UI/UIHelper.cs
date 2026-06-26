using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using telegramBot.src.Entities.Clothing;
using telegramBot.src.Entities.Session;

namespace telegramBot.src.UI
{
    internal static class UIHelper
    {
        public static async Task ClearOldMessagesAsync(ITelegramBotClient client, long chatId, UserSession session)
        {
            if (session.MessagesToDelete == null || session.MessagesToDelete.Count == 0) return;

            foreach (var messageId in session.MessagesToDelete)
            {
                try
                {
                    await client.DeleteMessage(chatId, messageId);
                }
                catch
                {
                    // Prevent crash
                }
            }
            session.MessagesToDelete.Clear();
        }

        // Reusable clothing album
        public static async Task RenderClothingAlbumAsync(ITelegramBotClient client, long chatId, UserSession session,
            List<ClothingItem> items, CancellationToken cancellationToken)
        {
            await ClearOldMessagesAsync(client, chatId, session);

            if (items.Count == 1)
            {
                var item = items[0];
                var singleMessage = await client.SendPhoto(
                    chatId: chatId,
                    photo: InputFile.FromFileId(item.FileId),
                    caption: $"№1: {item.Name}",
                    cancellationToken: cancellationToken
                );
                session.MessagesToDelete.Add(singleMessage.MessageId);
            }
            else
            {
                var media = items.Select((item, index) => new InputMediaPhoto(InputFile.FromFileId(item.FileId))
                {
                    Caption = $"№{index + 1}: {item.Name}"
                }).ToArray();

                var albumMessages = await client.SendMediaGroup(chatId, media, cancellationToken: cancellationToken);
                session.MessagesToDelete.AddRange(albumMessages.Select(m => m.MessageId));
            }

            ///keyboards
            //nav row
            var navigationRow = new List<KeyboardButton>();
            if (session.CurrentPage > 0) navigationRow.Add(new KeyboardButton("⬅️ Назад"));
            if (items.Count == 10) navigationRow.Add(new KeyboardButton("➡ Вперёд"));

            //action grid
            var actionRows = new List<KeyboardButton[]>();
            string buttonPrefix = session.Mode == SessionMode.RemoveItem ? "Удалить №" : "Выбрать №";
            var actionButtons = items.Select((_, i) => new KeyboardButton($"{buttonPrefix}{i + 1}")).ToArray();

            
            for (int i = 0; i < actionButtons.Length; i += 3)
            {
                actionRows.Add(actionButtons.Skip(i).Take(3).ToArray());
            }

            if (navigationRow.Count > 0)
            {
                actionRows.Insert(0, navigationRow.ToArray());
            }
            
            var replyKeyboard = new ReplyKeyboardMarkup(actionRows) { ResizeKeyboard = true };

            
            var keyboardMessage = await client.SendMessage(
                chatId,
                "Выберите нужный пункт в меню:",
                replyMarkup: replyKeyboard,
                cancellationToken: cancellationToken);

            session.MessagesToDelete.Add(keyboardMessage.MessageId);
        }
    }
}