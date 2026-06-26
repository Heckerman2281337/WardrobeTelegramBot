namespace telegramBot.src.Entities.Clothing
{
    public class ClothingItem
    {
        public ClothingItem() { }

        public ClothingItem(string name, long userId, string fileId, ClothingItemType clothingType) 
        { 
            Name = name;
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            UserId = userId;
            FileId = fileId;
            ClothingType = clothingType;
        }
        public string Name { get; private set; } = null!;
        public DateTime Created { get; private set; } 
        public Guid Id { get; private set; }
        public long UserId { get; private set; }
        public string FileId { get; private set; } = null!;
        public ClothingItemType ClothingType { get; private set; }
    }
}
