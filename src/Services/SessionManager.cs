using System.Collections.Concurrent;
using telegramBot.src.Entities;
namespace telegramBot.src.Services
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<long, UserSession> _sessions = new();

        private bool IsSessionExists(long userId)
        {
            if (!_sessions.ContainsKey(userId)) return false;

            return true;
        }

        public UserSession? GetSession(long userId)
        {
            _sessions.TryGetValue(userId, out var session);

            return session;
        }

        public void SetSession(long userId)
        {
            if (!IsSessionExists(userId))
                _sessions[userId] = new UserSession { Step = SessionStep.AddingType };
        }

        public void ClearSession(long userId)
        {
            if(IsSessionExists(userId)) _sessions.TryRemove(userId, out var userSession);
        }
    }
}
