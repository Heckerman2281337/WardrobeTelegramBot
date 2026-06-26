using Telegram.Bot.Types;
using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Services
{
    internal interface IClothingService
    {
        public Task AddClothingAsync(string name, long userId, string fileId, ClothingItemType type, CancellationToken cancellationToken);
        public Task<ClothingItem> GetItemAsync(Guid id, CancellationToken cancellationToken);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page, CancellationToken cancellationToken);
        public Task DeleteItemAsync(Guid id, CancellationToken cancellationToken);
    }
}
