using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSetupEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Setups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Timeframe = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    StopLoss = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    TakeProfit = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    PositionSize = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: true),
                    TradingThesis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Mood = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConfidenceLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Setups_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Setups_TradingStrategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "TradingStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetupChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrategyRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsMet = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetupChecklistItems_Setups_SetupId",
                        column: x => x.SetupId,
                        principalTable: "Setups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetupChecklistItems_StrategyRules_StrategyRuleId",
                        column: x => x.StrategyRuleId,
                        principalTable: "StrategyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetupImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetupImages_Setups_SetupId",
                        column: x => x.SetupId,
                        principalTable: "Setups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetupChecklistItemImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetupChecklistItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupChecklistItemImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetupChecklistItemImages_SetupChecklistItems_SetupChecklistItemId",
                        column: x => x.SetupChecklistItemId,
                        principalTable: "SetupChecklistItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetupChecklistItemImages_SetupChecklistItemId",
                table: "SetupChecklistItemImages",
                column: "SetupChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SetupChecklistItems_SetupId",
                table: "SetupChecklistItems",
                column: "SetupId");

            migrationBuilder.CreateIndex(
                name: "IX_SetupChecklistItems_StrategyRuleId",
                table: "SetupChecklistItems",
                column: "StrategyRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SetupImages_SetupId",
                table: "SetupImages",
                column: "SetupId");

            migrationBuilder.CreateIndex(
                name: "IX_Setups_StrategyId",
                table: "Setups",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Setups_UserId",
                table: "Setups",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetupChecklistItemImages");

            migrationBuilder.DropTable(
                name: "SetupImages");

            migrationBuilder.DropTable(
                name: "SetupChecklistItems");

            migrationBuilder.DropTable(
                name: "Setups");
        }
    }
}
