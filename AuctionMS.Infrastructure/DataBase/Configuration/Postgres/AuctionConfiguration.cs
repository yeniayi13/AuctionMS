
using Google.Api.Gax.ResourceNames;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction.ValueObjects;

namespace AuctionMS.Infrastructure.Database.Configuration.Postgres
{
    public class ProductConfiguration : IEntityTypeConfiguration<AuctionEntity>
    {
        public void Configure(EntityTypeBuilder<AuctionEntity> builder)
        {


            builder.ToTable("Auction");
            builder.HasKey(s => s.ProductId);
            builder.Property(s => s.ProductId)
                    .HasConversion(productId => productId.Value, value => ProductId.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.ProductName)
                    .HasConversion(productName => productName.Value, value => ProductName.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.ProductDescription)
                    .HasConversion(productDescription => productDescription.Value, value => ProductDescription.Create(value)!)
                    .IsRequired();

            builder.Property(s => s.ProductImage)
                  .HasConversion(productImage => productImage.Url, value => ProductImage.Create(value)!)
                  .IsRequired();

            builder.Property(s => s.ProductStock)
                  .HasConversion(productStock => productStock.Value, value => ProductStock.Create(value)!)
                  .IsRequired();

            builder.Property(s => s.ProductPrice)
                                .HasConversion(productPrice => productPrice.Value, value => ProductPrice.Create(value)!)
                                .IsRequired();

            builder.Property(s => s.ProductAvilability)
                    .HasConversion<string>()
                    .IsRequired();
            builder.Property(p => p.CategoryId)
                   .IsRequired(); // La clave foránea no puede ser nula

            builder.HasOne(p => p.Category)
.WithMany(c => c.Products)
.HasForeignKey(p => p.CategoryId)
.OnDelete(DeleteBehavior.Cascade) // Ajusta según tu lógica de negocio
.IsRequired();
            builder.Property(s => s.ProductUserId)
                               .HasConversion(ProductUserId => ProductUserId.Value, value => ProductUserId.Create(value)!)
                               .IsRequired();

        }
    }
}