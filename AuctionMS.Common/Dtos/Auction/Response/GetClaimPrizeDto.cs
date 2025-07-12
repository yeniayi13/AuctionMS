using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Response
{
    public class GetClaimPrizeDto
    {
        public Guid ClaimId { get; set; }
        public string ProductName { get; set; } = default!;
        public DateTime ClaimDate { get; set; }

        public string ShippingAddress { get; set; } = default!;
        public string DeliveryMethod { get; set; } = default!;
        
    }

}
