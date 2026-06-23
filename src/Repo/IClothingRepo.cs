using telegramBot.src.Entities.Clothing;
namespace telegramBot.src.Repo
{
    internal interface IClothingRepo
    {
        public Task<ClothingItem> AddClothingAsync(ClothingItem item);
        public Task<ClothingItem?> GetItemAsync(Guid id);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page);
        public Task DeleteItemAsync(ClothingItem item);

    }
}
