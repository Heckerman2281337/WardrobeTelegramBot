using Microsoft.Extensions.DependencyInjection;
using telegramBot.src.Handlers.Command;
using telegramBot.src.Handlers.Flow;

namespace telegramBot.src.Handlers
{
    public static class Extensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<IFlowHandler, AddFlowHandler>();
            services.AddScoped<IFlowHandler, RemoveFlowHandler>();
            services.AddScoped<IFlowHandler, OutfitFlowHandler>();

            services.AddScoped<BotHandler>();
            services.AddScoped<FlowRouter>();
            services.AddScoped<CommandHandler>();

            return services;
        }
    }
}
