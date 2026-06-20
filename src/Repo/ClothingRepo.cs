using telegramBot.src.Entities;

namespace telegramBot.src.Repo
{
    internal class ClothingRepo : IClothingRepo
    {
        public Task AddClothingAsync(ClothingItem item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteItemAsync(ClothingItem item)
        {
            throw new NotImplementedException();
        }

        public Task<ClothingItem> GetItemByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int skip, int take)
        {
            throw new NotImplementedException();
        }
    }
}
