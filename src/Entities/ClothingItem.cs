using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegramBot.src.Entities
{
    public class ClothingItem
    {
        public ClothingItem(long userId, string fileId, ClothingItemType clothingType) 
        { 
            Id = Guid.NewGuid();
            UserId = userId;
            FileId = fileId;
            ClothingType = clothingType;
        }

        public Guid Id { get; }
        public long UserId { get; }
        public string FileId { get; }
        public ClothingItemType ClothingType { get; }
    }
}
