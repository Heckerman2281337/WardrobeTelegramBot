using Microsoft.Extensions.DependencyInjection;

namespace telegramBot.src.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IClothingService, ClothingService>();

            return services;
        }
    }
}
