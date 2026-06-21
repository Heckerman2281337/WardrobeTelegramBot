using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using telegramBot.src.Repo;

namespace telegramBot.src;

//Factory for ef. desigin time. Mannually telling ef about dbcontext so it can create migrations :P
//Local dev DB, not production.
public class ClothingDbContextFactory : IDesignTimeDbContextFactory<ClothingDbContext>
{
    public ClothingDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Host=localhost;Database=WardrobeTelegramBot;Username=postgres;Password=12345";

        var options = new DbContextOptionsBuilder<ClothingDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ClothingDbContext(options);
    }
}