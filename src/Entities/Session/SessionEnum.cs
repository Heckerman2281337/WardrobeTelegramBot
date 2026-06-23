namespace telegramBot.src.Entities.Session
{
    public enum SessionStep
    {
        //Adding step
        AddingType,
        AddingPhoto,
        AddingName,

        //pick outfit step
        ChoosingOutfitTop,
        ChoosingOutfitBottom,
        ChoosingOutfitFootwear,
        ChoosingOutfitHeadwear,
        ChoosingOutfitFullBody,

        //Delete
        RemoveItem
    }

    public enum SessionMode
    {
        AddItem,
        RemoveItem,
        CreateOutfit
    }
}
