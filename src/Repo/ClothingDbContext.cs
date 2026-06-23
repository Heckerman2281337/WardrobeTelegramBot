using Microsoft.EntityFrameworkCore;
using telegramBot.src.Entities.Clothing;
namespace telegramBot.src.Repo
{
    public class ClothingDbContext(DbContextOptions<ClothingDbContext> options) : DbContext(options)
    {
        public DbSet<ClothingItem> ClothingItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClothingDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
