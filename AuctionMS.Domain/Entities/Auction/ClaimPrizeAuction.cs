using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction
{
    public class ClaimPrizeAuction
    {
        public enum DeliveryMethodType
        {
            EnvioEstandar,
            EnvioExpress,
         
        }

        public Guid AuctionId { get; set; }
        public Guid BidId { get; set; }
        public Guid UserId { get; set; }

        public string ShippingAddress { get; set; } = default!;
        public DeliveryMethodType DeliveryMethod { get; set; }
        public string ContactPhone { get; set; } = default!;
        public string AdditionalNotes { get; set; } = string.Empty;


    }

}
