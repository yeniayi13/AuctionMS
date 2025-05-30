using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Response
{
    public class GetProduct
    {
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? ProductImage { get; init; }
        public decimal? ProductPrice { get; init; }
        public string? ProductDescription { get; init; }
        public string? ProductAvilability { get; init; }
        public decimal ProductStock { get; set; }
        public Guid CategoryId { get; init; }
        
        public Guid ProductUserId { get; init; }



    }
}
