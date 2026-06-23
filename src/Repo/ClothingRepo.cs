using Microsoft.EntityFrameworkCore;
using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Repo
{
    internal class ClothingRepo : IClothingRepo
    {
        private const int PageSize = 10;
        public ClothingRepo(ClothingDbContext context) 
        { 
            _context = context;
        }
        private readonly ClothingDbContext _context;

        public async Task<ClothingItem> AddClothingAsync(ClothingItem item)
        {
            await _context.ClothingItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(ClothingItem item)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page)
        {
            var skip = (page - 1) * PageSize;

            return await _context.ClothingItems
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.ClothingType == type)
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<ClothingItem?> GetItemAsync(Guid id)
        {
            return await _context.ClothingItems.FindAsync(id);
        }
    }
}
