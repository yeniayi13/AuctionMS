using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace AuctionMS.Common.Dtos.Auction.Response
{
    public class GetAuctionDto
    {
        public Guid AuctionId { get; init; }
        public string? AuctionName { get; init; }
        public string? AuctionImage { get; init; }
        public decimal AuctionPriceBase { get; init; }
        public decimal AuctionPriceReserva { get; init; }
        public string? AuctionDescription { get; init; }
        public decimal AuctionIncremento { get; init; }
        public DateTime AuctionFechaInicio { get; init; }
        public DateTime AuctionFechaFin { get; init; }
        public string? AuctionCondiciones { get; init; }
        public decimal AuctionCantidadProducto { get; init; }
        public Guid AuctionUserId { get; init; }
        public Guid AuctionProductId { get; init; }
        public Guid AuctionBidId { get; init; }

      
        public string? AuctionCurrentState { get; init; }
    }



}
