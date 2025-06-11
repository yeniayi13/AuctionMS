using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Request
{
    public record CreateAuctionDto
    {
        public Guid AuctionId { get; init; } = Guid.NewGuid(); // Genera un nuevo GUID por defecto
        public string? AuctionName { get; init; }
        public string? AuctionImage { get; init; }
        public decimal AuctionPriceBase { get; init; }
        public decimal AuctionPriceReserva { get; init; }
        public string? AuctionDescription { get; init; }

        public decimal AuctionIncremento { get; init; }

        public DateTime AuctionFechaInicio { get; init; }
        public DateTime AuctionFechaFin { get; init; }
        public string? AuctionCondiciones { get; init; }

        public int AuctionCantidadProducto { get; init; }

        public Guid AuctionUserId { get; init; }
        public Guid AuctionProductId { get; init; }// Genera un nuevo GUID por defecto
    }
}

