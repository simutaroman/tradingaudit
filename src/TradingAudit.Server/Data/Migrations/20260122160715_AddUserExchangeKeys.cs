using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExchangeKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserExchangeKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExchangeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApiSecret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExchangeKeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserExchangeKeys_UserId",
                table: "UserExchangeKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExchangeKeys_UserId_ExchangeName_ApiKey",
                table: "UserExchangeKeys",
                columns: new[] { "UserId", "ExchangeName", "ApiKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserExchangeKeys");
        }
    }
}
