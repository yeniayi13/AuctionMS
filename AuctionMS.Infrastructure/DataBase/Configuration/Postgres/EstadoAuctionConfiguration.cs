using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AuctionMS.Domain.Entities.Auction;

namespace AuctionMS.Infrastructure.DataBase.Configuration.Postgres
{

    public class EstadoOrdenSagaConfiguration : IEntityTypeConfiguration<EstadoAuction>
    {
        public void Configure(EntityTypeBuilder<EstadoAuction> builder)
        {
            builder.ToTable("EstadoAuction");

            builder.HasKey(x => x.CorrelationId);

            // CorrelationId es clave primaria, usualmente no se genera automáticamente
            builder.Property(x => x.CorrelationId)
                   .ValueGeneratedNever()
                   .IsRequired();

            builder.Property(x => x.CurrentState)
                   .IsRequired()
                   .HasMaxLength(64);


            builder.Property(x => x.FechaInicio)
                   .IsRequired();

     
            builder.HasIndex(x => x.CurrentState);
        }
    }

}
