using telegramBot.src.Entities.Clothing;
namespace telegramBot.src.Repo
{
    internal interface IClothingRepo
    {
        public Task<ClothingItem> AddClothingAsync(ClothingItem item, CancellationToken cancellationToken);
        public Task<ClothingItem?> GetItemAsync(Guid id, CancellationToken cancellationToken);
        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page, CancellationToken cancellationToken);
        public Task DeleteItemAsync(ClothingItem item, CancellationToken cancellationToken);

    }
}
