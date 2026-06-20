using telegramBot.src.Entities;

namespace telegramBot.src.Services
{
    internal interface IClothingService
    {
        public Task AddClothingAsync(ClothingItem item);
        public Task<ClothingItem> GetItemByIdAsync(Guid id);
        public Task<List<ClothingItem>> GetItemByTypeAsync(ClothingItemType type);
        public Task DeleteItemAsync(Guid id);
    }
}
