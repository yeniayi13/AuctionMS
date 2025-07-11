using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Common.Dtos.Auction.Response
{
    public class GetPaymentDto
    {
        public Guid PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public Guid PaymentCardId { get; set; }
        public string PaymentStatus { get; set; }
        public Guid PaymentUserId { get; set; }
        public Guid PaymentAuctionId { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
