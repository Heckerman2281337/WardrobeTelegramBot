using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
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
            var bot = new TelegramBotClient(token);
            var me = await bot.GetMe();
            var handler = new BotHandler(bot);
            bot.OnUpdate += OnUpdate;

            //Dev
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            Console.ReadLine();

            //await Task.Delay(-1); Prod

            async Task OnUpdate(Update update)
            {
                await handler.HandleUpdateAsync(update, cts.Token);
            }
        }
    }
}
