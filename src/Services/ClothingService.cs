using telegramBot.src.Entities.Clothing;
using telegramBot.src.Repo;
using telegramBot.src.Services;

namespace telegramBot.src
{
    internal class ClothingService : IClothingService
    {
        public ClothingService(IClothingRepo repo)
        {
            _repo = repo;
        }

        private readonly IClothingRepo _repo;
        public async Task AddClothingAsync(string name, long userId, string fileId, ClothingItemType type)
        {
            var item = new ClothingItem(name.ToLower(), userId, fileId, type);
            
            await _repo.AddClothingAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var item = await GetItemAsync(id);

            if (item == null) return;

            await _repo.DeleteItemAsync(item);
        }

        public async Task<ClothingItem> GetItemAsync(Guid id)
        {
            var item = _repo.GetItemAsync(id);

            if (item == null) throw new ArgumentException($"Item {id} not found");

            return await item;
        }

        public async Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page)
        {
            return await _repo.GetItemByTypeAsync(userId, type, page);
        }

        
    }
}
