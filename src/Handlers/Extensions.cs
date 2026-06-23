using Microsoft.Extensions.DependencyInjection;

namespace telegramBot.src.Handlers
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<BotHandler>();
            services.AddScoped<BotUserFlowHandler>();
            services.AddScoped<BotCommandHandler>();

            return services;
        }
    }
}
