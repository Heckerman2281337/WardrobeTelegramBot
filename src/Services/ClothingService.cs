using Telegram.Bot.Types;
using telegramBot.src.Entities;
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
            var item = new ClothingItem(name, userId, fileId, type);
            
            await _repo.AddClothingAsync(item);
        }

        public Task DeleteItemAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ClothingItem> GetItemByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClothingItem>> GetItemByTypeAsync(ClothingItemType type)
        {
            throw new NotImplementedException();
        }
    }
}
