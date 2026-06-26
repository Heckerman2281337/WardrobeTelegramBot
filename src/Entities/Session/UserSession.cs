using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Entities.Session
{
    public class UserSession
    {
        public SessionMode? Mode { get; set; }
        public SessionStep? Step { get; set; }

        public ClothingItemType? Type { get; set; }

        public string? ClothingName { get; set; }
        public string? FileId { get; set; }
        
        public Guid? SelectedItemId { get; set; } 
        
        public ClothingItemType? CurrentOutfitChoice { get; set; }
        
        public OutfitDraft? OutfitDraft { get; set; }

        public int CurrentPage { get; set; } = 0;

        public List<int> MessagesToDelete { get; set; } = new();
    }
}
