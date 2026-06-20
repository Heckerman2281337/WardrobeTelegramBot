using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using telegramBot.src.Repo;
namespace telegramBot.src
{
    public class Program
    {
        public static async Task Main()
        {
            DotNetEnv.Env.Load();
            string? token = Environment.GetEnvironmentVariable("TOKEN");
            if (token == null) throw new ArgumentException("Токен бота не найден в .env");

            using var cts = new CancellationTokenSource();
            
            // host for Runtime cycle of bot
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDataAcces();

                    services.AddScoped<BotHandler>();
                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
                }
                )
                .Build();

            using var scope = host.Services.CreateScope();

            var bot = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var me = await bot.GetMe();
            //bot.OnUpdate += OnUpdate;

            var handler = scope.ServiceProvider
                .GetRequiredService<BotHandler>();

            // Dev debugging
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            Console.ReadLine();

            //await Task.Delay(-1); Prod 
            /*
            async Task OnUpdate(Update update)
            {
                await handler.HandleUpdateAsync(update, cts.Token);
            }
            */
        }
    }
}
