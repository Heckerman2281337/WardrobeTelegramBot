# !!CURRENTLY NOT DEPLOYED CAUSE IM BROKE!!
# WardrobeTelegramBot
Project that I created for my gf. She had a trouble with her wardrobe.

## Stack
### Backend
- C# 
- .NET 9
- .NET Generic Host
- EF Core
### DB
- Postgres
### Infrastructure
- Docker
### Other
- Telegram.Bot (Polling)

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
- [x] Dockerize the application (Bot + Database).
- [ ] Refactor removeflow and outfitflow
- [ ] Maybe add English lang for it
- [ ] Maybe add AI to create outfit image?
- [x] Host on railway
- [ ] Make commands more comfortable
- [ ] Add timeout for sessions
