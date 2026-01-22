using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddExecutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExecutionId",
                table: "Setups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Executions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealizedPnL = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    NetPnL = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    TotalFunding = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    ROI = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AvgEntryPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    AvgExitPrice = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    SlippagePoints = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    SlippagePercent = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BreakevenTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Executions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Executions_Setups_SetupId",
                        column: x => x.SetupId,
                        principalTable: "Setups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Side = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    ExecutedAmount = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeOrders_Executions_ExecutionId",
                        column: x => x.ExecutionId,
                        principalTable: "Executions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderFills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalFillId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExchangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    CommissionAsset = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "USDT"),
                    TradeTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFills_ExchangeOrders_ExchangeOrderId",
                        column: x => x.ExchangeOrderId,
                        principalTable: "ExchangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_ExecutionId",
                table: "ExchangeOrders",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_ExternalOrderId_UserId",
                table: "ExchangeOrders",
                columns: new[] { "ExternalOrderId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_UserId",
                table: "ExchangeOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Executions_SetupId",
                table: "Executions",
                column: "SetupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderFills_ExchangeOrderId",
                table: "OrderFills",
                column: "ExchangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFills_ExternalFillId",
                table: "OrderFills",
                column: "ExternalFillId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderFills");

            migrationBuilder.DropTable(
                name: "ExchangeOrders");

            migrationBuilder.DropTable(
                name: "Executions");

            migrationBuilder.DropColumn(
                name: "ExecutionId",
                table: "Setups");
        }
    }
}
