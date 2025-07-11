using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Response
{
    public record class GetBid
    {
        public Guid BidId { get; set; }
        public Guid BidAuctionId { get; set; }
        public Guid BidUserId { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountMax { get; set; } // For max bid type
        public decimal Increment { get; set; } // Increment for automatic bids
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime BidTime { get; set; }
    }
}
