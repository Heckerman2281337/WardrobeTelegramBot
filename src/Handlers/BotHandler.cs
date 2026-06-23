using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using telegramBot.src.Services;

namespace telegramBot.src.Handlers
{
    internal class BotHandler
    {

        public BotHandler(ITelegramBotClient botClient, SessionManager sessionManager,
            BotCommandHandler commandHandler, BotUserFlowHandler flowHandler) 
        {
            _client = botClient;
            _sessionManager = sessionManager;
            _commandHandler = commandHandler;
            _flowHandler = flowHandler;
        }

        private readonly ITelegramBotClient _client;
        private readonly SessionManager _sessionManager;
        private readonly BotCommandHandler _commandHandler;
        private readonly BotUserFlowHandler _flowHandler;

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
                    cancellationToken: cancellationToken);
                return;
            }

            var session = _sessionManager.GetSession(userId);

            if (session == null) await _commandHandler.HandleCommandAsync(message, cancellationToken);
            else await _flowHandler.HandleFlowAsync(message, userId, cancellationToken);
        }
    }
}
