using telegramBot.src.Entities;

namespace telegramBot.src.Repo
{
    internal class ClothingRepo : IClothingRepo
    {
        public ClothingRepo(ClothingDbContext context) 
        { 
            _context = context;
        }
        private readonly ClothingDbContext _context;

        public async Task<ClothingItem> AddClothingAsync(ClothingItem item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public Task DeleteItemAsync(ClothingItem item)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int skip, int take)
        {
            throw new NotImplementedException();
        }
    }
}
