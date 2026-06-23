using telegramBot.src.Services;
using Telegram.Bot;
using telegramBot.src.Entities.Session;
using Telegram.Bot.Types;

namespace telegramBot.src.Handlers.Flow
{   //Handler for existing session
    internal class FlowRouter
    {
        public FlowRouter(ITelegramBotClient client, SessionManager sessionManager, IEnumerable<IFlowHandler> flowHandler) 
        { 
            _client = client;
            _sessionManager = sessionManager;
            _flowHandler = flowHandler;
        }
        private readonly ITelegramBotClient _client;
        private readonly SessionManager _sessionManager;
        private readonly IEnumerable<IFlowHandler> _flowHandler;

        public async Task RouteFlowAsync(Message message, long userId, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(userId);
            if (session == null) return;

            var targetHandler = _flowHandler.FirstOrDefault(h => h.Mode == session.Mode);

            if (targetHandler == null) return;

            await targetHandler.HandleAsync(session, message, cancellationToken);
        }
    }
}
