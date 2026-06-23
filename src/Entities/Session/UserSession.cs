using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Entities.Session
{
    public class UserSession
    {
        public string? ClothingName { get; set; }
        public string? FileId { get; set; }
        public SessionStep? Step { get; set; }
        public ClothingItemType? Type { get; set; }
        public SessionMode? Mode { get; set; }
    }
}
