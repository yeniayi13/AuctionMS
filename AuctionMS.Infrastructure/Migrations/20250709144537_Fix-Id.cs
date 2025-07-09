using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionBidId_Value",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "AuctionPaymentId",
                table: "Auctions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuctionBidId_Value",
                table: "Auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionPaymentId",
                table: "Auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
