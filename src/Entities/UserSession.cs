namespace telegramBot.src.Entities
{
    public class UserSession
    {
        public string? ClothingName { get; set; }
        public string? FileId { get; set; }
        public SessionStep? Step { get; set; }
        public ClothingItemType? Type { get; set; }

    }
}
