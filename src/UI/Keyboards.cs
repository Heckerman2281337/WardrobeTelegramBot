using Telegram.Bot.Types.ReplyMarkups;
public static class Keyboards
{
    public static ReplyMarkup ClothingTypes =>
        new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Вверх", "Низ" },
            new KeyboardButton[] { "Обувь", "Голова" },
            new KeyboardButton[] { "Всё тело" }
        })
        {
            ResizeKeyboard = true
        };
}