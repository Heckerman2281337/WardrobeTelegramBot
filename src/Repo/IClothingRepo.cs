using telegramBot.src.Entities;
namespace telegramBot.src.Repo
{
    internal interface IClothingRepo
    {
        public Task<ClothingItem> AddClothingAsync(ClothingItem item);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int skip, int take);
        public Task DeleteItemAsync(ClothingItem item);

    }
}
