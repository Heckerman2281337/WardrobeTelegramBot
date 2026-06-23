using Telegram.Bot.Types;
using telegramBot.src.Entities.Clothing;
using telegramBot.src.Entities.Session;

namespace telegramBot.src.Handlers.Flow
{
    internal interface IFlowHandler
    {
        SessionMode Mode { get; }
        Task HandleAsync(UserSession session, Message message, CancellationToken cancellationToken);
    };

    public static class ClothingMap
    {
        public static readonly Dictionary<string, ClothingItemType> Types = new()
        {
            { "Вверх", ClothingItemType.Top },
            { "Низ", ClothingItemType.Bottom },
            { "Обувь", ClothingItemType.Footwear },
            { "Голова", ClothingItemType.Headwear },
            { "Всё тело", ClothingItemType.FullBody }
        };
    }
}
