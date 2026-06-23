using Telegram.Bot.Types;
using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Services
{
    internal interface IClothingService
    {
        public Task AddClothingAsync(string name, long userId, string fileId, ClothingItemType type);
        public Task<ClothingItem> GetItemAsync(Guid id);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page);
        public Task DeleteItemAsync(Guid id);
    }
}
