using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace telegramBot.src.Migrations
{
    /// <inheritdoc />
    public partial class ClothingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClothingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<string>(type: "text", nullable: false),
                    ClothingType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothingItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothingItems_Id",
                table: "ClothingItems",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothingItems");
        }
    }
}
