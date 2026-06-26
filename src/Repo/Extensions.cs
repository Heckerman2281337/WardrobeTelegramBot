using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace telegramBot.src.Repo
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcces(this IServiceCollection services)
        {
            services.AddScoped<IClothingRepo, ClothingRepo>();
            services.AddDbContext<ClothingDbContext>(x =>
            {
                x.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"));
            });

            return services;
        }
    }
}
