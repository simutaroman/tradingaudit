using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTradingStrategies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradingStrategies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultRiskAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinRiskRewardRatio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timeframes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetScope = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LifecycleStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingStrategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradingStrategies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StrategyImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategyImages_TradingStrategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "TradingStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StrategyRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StrategyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    LogicParameter = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategyRules_TradingStrategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "TradingStrategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StrategyRuleImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StrategyRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyRuleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategyRuleImages_StrategyRules_StrategyRuleId",
                        column: x => x.StrategyRuleId,
                        principalTable: "StrategyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StrategyImages_StrategyId",
                table: "StrategyImages",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRuleImages_StrategyRuleId",
                table: "StrategyRuleImages",
                column: "StrategyRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyRules_StrategyId",
                table: "StrategyRules",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingStrategies_GroupId",
                table: "TradingStrategies",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingStrategies_GroupId_Version",
                table: "TradingStrategies",
                columns: new[] { "GroupId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradingStrategies_UserId",
                table: "TradingStrategies",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StrategyImages");

            migrationBuilder.DropTable(
                name: "StrategyRuleImages");

            migrationBuilder.DropTable(
                name: "StrategyRules");

            migrationBuilder.DropTable(
                name: "TradingStrategies");
        }
    }
}
