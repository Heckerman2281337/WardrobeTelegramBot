![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![Telegram API](https://img.shields.io/badge/Telegram_API-2CA5E0?style=flat&logo=telegram)
# WardrobeTelegramBot

## Stack
- Language: C# 
- Framework: .NET (Worker and Hosted services) and EF Core
- API: Telegram.Bot (Polling)
- Database: Postgres
## Description
Tired of trying to find your perfect outfit? And putting all those clothes away afterward is a real pain!

**WardrobeTelegramBot** is your personal digital closet assistant. It helps you digitize your wardrobe so you can easily find, mix, and match your clothes right from your phone.

## What can it do?
- **Digitize your clothes:** Just send a photo of your clothing item to the bot.
- **Categorize & Name:** Assign a type (e.g. *Bottom*, *Top*, *Shoes*) and a custom name (e.g. *Favorite Blue Jeans*).
- **Browse your looks:** Scroll through your digital wardrobe whenever you need to pick an outfit!

> **Privacy First:** Dont worry about photo leaks. All media is securely stored on Telegrams servers. The bot only stores the unique `file_id` of the media, meaning no personal photos are downloaded to database.

## How to use it?
1. Start a chat with the bot in Telegram (Bots username: @wardrobe67_bot).
2. Send the `/start` command to initialize your wardrobe.
3. Send a photo of any clothing item.
4. Follow the bots prompts to select the category and set a name for the item.

## Local Setup
1. Clone the repository.
2. Create a `.env` file in the root directory and specify your Telegram Bot token:
   `TOKEN=your_token_here`
3. Configure the PostgreSQL connection string in `appsettings.json`.
4. Apply EF Core migrations to set up the database schema:
   `dotnet ef database update`
5. Run the application:
   `dotnet run`

## Notes for me (TODO)
- [ ] Add structured logger
- [ ] Dockerize the application (Bot + Database).

- [ ] Maybe add English lang for it
- [ ] Maybe add AI to create outfit image?
