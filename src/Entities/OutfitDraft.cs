using telegramBot.src.Entities.Clothing;

namespace telegramBot.src.Entities
{
    public class OutfitDraft
    {
        public List<ClothingItem> Items { get; set; } = new();
    }
}
