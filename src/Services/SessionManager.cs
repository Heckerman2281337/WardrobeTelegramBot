using System.Collections.Concurrent;
using telegramBot.src.Entities.Session;
namespace telegramBot.src.Services
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<long, UserSession> _sessions = new();

        public UserSession? GetSession(long userId)
        {
            _sessions.TryGetValue(userId, out var session);

            return session;
        }

        public void SetSession(long userId, SessionMode sessionMode)
        {
            _sessions[userId] = sessionMode switch
            {
                SessionMode.AddItem => new UserSession
                {
                    Step = SessionStep.AddingType,
                    Mode = sessionMode
                },

                SessionMode.RemoveItem => new UserSession
                {
                    Step = SessionStep.RemoveItem,
                    Mode = sessionMode
                },

                SessionMode.CreateOutfit => new UserSession
                {
                    Step = SessionStep.ChoosingOutfitTop,
                    Mode = sessionMode
                },

                _ => new UserSession()
            };
        }

        public void ClearSession(long userId)
        {
            _sessions.TryRemove(userId, out var userSession);
        }
    }
}
