using System;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionName = table.Column<string>(type: "text", nullable: false),
                    AuctionImage = table.Column<string>(type: "text", nullable: false),
                    AuctionPriceBase = table.Column<decimal>(type: "numeric", nullable: false),
                    AuctionPriceReserva = table.Column<decimal>(type: "numeric", nullable: false),
                    AuctionDescription = table.Column<string>(type: "text", nullable: false),
                    AuctionIncremento = table.Column<decimal>(type: "numeric", nullable: false),
               

                    AuctionCondiciones = table.Column<string>(type: "text", nullable: false),
                    AuctionUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.AuctionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auction");
        }
    }
}
