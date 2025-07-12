
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionBidId
    {
        private AuctionBidId(Guid value) => Value = value;

        public static AuctionBidId Create()
        {
            return new AuctionBidId(Guid.NewGuid());
        }
        public static AuctionBidId? Create(Guid value)
        {
            //if (value == Guid.Empty) throw new NullAttributeException("Auction userId is required");

            return new AuctionBidId(value);
        }

        public static AuctionBidId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
