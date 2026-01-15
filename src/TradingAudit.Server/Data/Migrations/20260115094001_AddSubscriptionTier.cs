using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingAudit.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionExpiresAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionTier",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionTier",
                table: "AspNetUsers");
        }
    }
}
