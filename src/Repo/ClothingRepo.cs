using Microsoft.EntityFrameworkCore;
using telegramBot.src.Entities;
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

        public async Task<ClothingItem> AddClothingAsync(ClothingItem item, CancellationToken cancellationToken)
        {
            await _context.ClothingItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(ClothingItem item, CancellationToken cancellationToken)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClothingItem>> GetItemByTypeAsync(long userId, ClothingItemType type, int page, CancellationToken cancellationToken)
        {
            var skip = page * PageSize;

            return await _context.ClothingItems
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.ClothingType == type)
                .Skip(skip)
                .Take(PageSize)
                .OrderByDescending(x => x.Created)
                .ToListAsync();
        }
        public async Task<ClothingItem?> GetItemAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.ClothingItems.FindAsync(id);
        }
    }
}
