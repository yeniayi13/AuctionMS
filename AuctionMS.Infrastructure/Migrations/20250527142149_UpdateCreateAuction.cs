using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreateAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionDuracion",
                table: "Auction");

            migrationBuilder.AddColumn<int>(
                name: "AuctionCantidadProducto",
                table: "Auction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuctionFechaFin",
                table: "Auction",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AuctionFechaInicio",
                table: "Auction",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionCantidadProducto",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "AuctionFechaFin",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "AuctionFechaInicio",
                table: "Auction");

            migrationBuilder.AddColumn<decimal>(
                name: "AuctionDuracion",
                table: "Auction",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
