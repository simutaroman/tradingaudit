using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class LinkOrdersToKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserExchangeKeyId",
                table: "ExchangeOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_UserExchangeKeyId",
                table: "ExchangeOrders",
                column: "UserExchangeKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeOrders_UserExchangeKeys_UserExchangeKeyId",
                table: "ExchangeOrders",
                column: "UserExchangeKeyId",
                principalTable: "UserExchangeKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeOrders_UserExchangeKeys_UserExchangeKeyId",
                table: "ExchangeOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeOrders_UserExchangeKeyId",
                table: "ExchangeOrders");

            migrationBuilder.DropColumn(
                name: "UserExchangeKeyId",
                table: "ExchangeOrders");
        }
    }
}
