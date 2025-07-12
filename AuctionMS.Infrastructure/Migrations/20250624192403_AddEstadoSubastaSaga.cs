using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoSubastaSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AuctionCantidadProducto",
                table: "Auctions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionBidId_Value",
                table: "Auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CurrentState",
                table: "Auctions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadoAuction",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoActual = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoAuction", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadoAuction_EstadoActual",
                table: "EstadoAuction",
                column: "EstadoActual");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadoAuction");

            migrationBuilder.DropColumn(
                name: "AuctionBidId_Value",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "CurrentState",
                table: "Auctions");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionCantidadProducto",
                table: "Auctions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
