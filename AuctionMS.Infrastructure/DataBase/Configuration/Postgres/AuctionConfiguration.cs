
using Google.Api.Gax.ResourceNames;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Infrastructure.Database.Configuration.Postgres
{
    public class AuctionConfiguration : IEntityTypeConfiguration<AuctionEntity>
    {
        public void Configure(EntityTypeBuilder<AuctionEntity> builder)
        {


            builder.ToTable("Auctions");
            builder.HasKey(s => s.AuctionId);
            builder.Property(s => s.AuctionId)
                    .HasConversion(auctionId => auctionId.Value, value => AuctionId.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.AuctionName)
                    .HasConversion(auctionName => auctionName.Value, value => AuctionName.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.AuctionDescription)
                    .HasConversion(auctionDescription => auctionDescription.Value, value => AuctionDescription.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.AuctionImage)
                  .HasConversion(auctionImage => auctionImage.Url, value => AuctionImage.Create(value)!)
                  .IsRequired();

            builder.Property(s => s.AuctionCondiciones)
                  .HasConversion(auctionCondiciones => auctionCondiciones.Value, value => AuctionCondiciones.Create(value)!)
                  .IsRequired();

            builder.Property(s => s.AuctionPriceBase)
                                .HasConversion(auctionPriceBase => auctionPriceBase.Value, value => AuctionPriceBase.Create(value)!)
                                .IsRequired();


            builder.Property(s => s.AuctionPriceReserva)
                                .HasConversion(auctionPriceReserva => auctionPriceReserva.Value, value => AuctionPriceReserva.Create(value)!)
                                .IsRequired();

            builder.Property(s => s.AuctionFechaInicio)
                              .HasConversion(auctionFechaInicio => auctionFechaInicio.Value, value => AuctionFechaInicio.Create(value)!)
                              .IsRequired();

            builder.Property(s => s.AuctionFechaFin)
                              .HasConversion(auctionFechaFin => auctionFechaFin.Value, value => AuctionFechaFin.Create(value)!)
                              .IsRequired();

            builder.Property(s => s.AuctionIncremento)
                             .HasConversion(auctionIncremento => auctionIncremento.Value, value => AuctionIncremento.Create(value)!)
                             .IsRequired();

            builder.Property(s => s.AuctionCantidadProducto)
                             .HasConversion(auctionCantidadProducto => auctionCantidadProducto.Value, value => AuctionCantidadProducto.Create(value)!)
                             .IsRequired();



            builder.Property(s => s.AuctionUserId)
                               .HasConversion(AuctionUserId => AuctionUserId.Value, value => AuctionUserId.Create(value)!)
                               .IsRequired();

            builder.Property(s => s.AuctionProductId)
                              .HasConversion(AuctionProductId => AuctionProductId.Value, value => AuctionProductId.Create(value)!)
                              .IsRequired();

        }
    }
}