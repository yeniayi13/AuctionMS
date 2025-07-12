using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AuctionCantidadProducto",
                table: "Auctions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionPaymentId",
                table: "Auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionPaymentId",
                table: "Auctions");

            migrationBuilder.AlterColumn<decimal>(
                name: "AuctionCantidadProducto",
                table: "Auctions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
