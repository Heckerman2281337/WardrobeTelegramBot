using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegramBot.src.Handlers.Command;
using telegramBot.src.Handlers.Flow;
using telegramBot.src.Services;

namespace telegramBot.src.Handlers
{
    internal class BotHandler
    {
        public BotHandler(ITelegramBotClient botClient, SessionManager sessionManager,
            CommandHandler commandHandler, FlowRouter flowRouter) 
        {
            _client = botClient;
            _sessionManager = sessionManager;
            _commandHandler = commandHandler;
            _flowRouter = flowRouter;
        }

        private readonly ITelegramBotClient _client;
        private readonly SessionManager _sessionManager;
        private readonly CommandHandler _commandHandler;
        private readonly FlowRouter _flowRouter;

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            if(update.Message == null) return;
            var message = update.Message;
            var userId = update.Message!.From!.Id;

            if (update.Message?.Type == MessageType.Text && update.Message.Text!.Trim().StartsWith("/cancel"))
            {
                _sessionManager.ClearSession(userId); //remove session if user write /cancel

                await _client.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "Операция отменена.",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
                return;
            }


            Console.WriteLine($"[Logging] Event: MessageReceived | UserID: {message.From?.Id} | ChatID: {message.Chat.Id}");

            var session = _sessionManager.GetSession(userId);
            if (session == null) await _commandHandler.HandleCommandAsync(message, cancellationToken);
            else await _flowRouter.RouteFlowAsync(message, userId, cancellationToken);
        }
    }
}
