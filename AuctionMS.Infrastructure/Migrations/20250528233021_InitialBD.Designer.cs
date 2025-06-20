﻿// <auto-generated />
using System;
using AuctionMS.Infrastructure.Database.Context.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuctionMS.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250528233021_InitialBD")]
    partial class InitialBD
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuctionMS.Domain.Entities.Auction.AuctionEntity", b =>
                {
                    b.Property<Guid>("AuctionId")
                        .HasColumnType("uuid");

                    b.Property<int>("AuctionCantidadProducto")
                        .HasColumnType("integer");

                    b.Property<string>("AuctionCondiciones")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AuctionDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("AuctionFechaFin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("AuctionFechaInicio")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AuctionImage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("AuctionIncremento")
                        .HasColumnType("numeric");

                    b.Property<string>("AuctionName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("AuctionPriceBase")
                        .HasColumnType("numeric");

                    b.Property<decimal>("AuctionPriceReserva")
                        .HasColumnType("numeric");

                    b.Property<Guid>("AuctionProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuctionUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("AuctionId");

                    b.ToTable("Auction", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
