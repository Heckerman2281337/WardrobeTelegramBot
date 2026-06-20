using telegramBot.src.Entities;
namespace telegramBot.src.Repo
{
    internal interface IClothingRepo
    {
        public Task AddClothingAsync(ClothingItem item);
        public Task<ClothingItem> GetItemByIdAsync(Guid id);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int skip, int take);
        public Task DeleteItemAsync(ClothingItem item);

    }
}
