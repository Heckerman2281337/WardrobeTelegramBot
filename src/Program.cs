using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telegramBot.src.Repo;
using telegramBot.src.Services;
using telegramBot.src.Handlers;
using Microsoft.EntityFrameworkCore;
namespace telegramBot.src
{
    public class Program
    {
        public static async Task Main()
        {
            DotNetEnv.Env.Load();
            string? token = Environment.GetEnvironmentVariable("TOKEN");
            if (token == null) throw new ArgumentException("Токен бота не найден в .env");

            var bot = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();
            
            // host for Runtime cycle of bot
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDataAcces();
                    services.AddBusinessLogic();
                    services.AddHandlers();
                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
                    services.AddSingleton<SessionManager>();
                    services.AddHostedService<BotBackgroundService>();
                }
                )
                .Build();
            
            // Auto migration (for deployment)
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClothingDbContext>();
                db.Database.Migrate();
            }

            await host.RunAsync();
        }
    }
}
