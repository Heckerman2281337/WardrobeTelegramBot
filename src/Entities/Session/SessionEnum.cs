namespace telegramBot.src.Entities.Session
{
    public enum SessionStep
    {
        SelectType,
        SelectPhoto,
        SelectName,

        SelectItem,
        ConfirmAction,
    }

    public enum SessionMode
    {
        AddItem,
        RemoveItem,
        CreateOutfit
    }
}
