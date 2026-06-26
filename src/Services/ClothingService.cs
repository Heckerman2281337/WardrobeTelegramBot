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
        public async Task AddClothingAsync(string name, long userId, string fileId, ClothingItemType type, CancellationToken cancellationToken)
        {
            var item = new ClothingItem(name.ToLower(), userId, fileId, type);
            
            await _repo.AddClothingAsync(item, cancellationToken);
        }

        public async Task DeleteItemAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = await GetItemAsync(id, cancellationToken);

            if (item == null) return;

            await _repo.DeleteItemAsync(item, cancellationToken);
        }

        public async Task<ClothingItem> GetItemAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = await _repo.GetItemAsync(id, cancellationToken);

            if (item == null) throw new ArgumentException($"Item {id} not found");

            return item;
        }

        public async Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page, CancellationToken cancellationToken)
        {
            return await _repo.GetItemByTypeAsync(userId, type, page, cancellationToken);
        }

        
    }
}
