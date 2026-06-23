using Telegram.Bot.Types;
using telegramBot.src.Entities;

namespace telegramBot.src.Services
{
    internal interface IClothingService
    {
        public Task AddClothingAsync(string name, long userId, string fileId, ClothingItemType type);
        public Task<ClothingItem> GetItemByIdAsync(Guid id);
        public Task<List<ClothingItem>> GetItemByTypeAsync(ClothingItemType type);
        public Task DeleteItemAsync(Guid id);
    }
}
