using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFkBidAuctionEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentState",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "UltimaActualizacion",
                table: "EstadoAuction",
                newName: "FechaInicio");

            migrationBuilder.RenameColumn(
                name: "EstadoActual",
                table: "EstadoAuction",
                newName: "CurrentState");

            migrationBuilder.RenameIndex(
                name: "IX_EstadoAuction_EstadoActual",
                table: "EstadoAuction",
                newName: "IX_EstadoAuction_CurrentState");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionId",
                table: "EstadoAuction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AuctionEstado",
                table: "Auctions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "EstadoAuction");

            migrationBuilder.DropColumn(
                name: "AuctionEstado",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "EstadoAuction",
                newName: "UltimaActualizacion");

            migrationBuilder.RenameColumn(
                name: "CurrentState",
                table: "EstadoAuction",
                newName: "EstadoActual");

            migrationBuilder.RenameIndex(
                name: "IX_EstadoAuction_CurrentState",
                table: "EstadoAuction",
                newName: "IX_EstadoAuction_EstadoActual");

            migrationBuilder.AddColumn<int>(
                name: "CurrentState",
                table: "Auctions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
