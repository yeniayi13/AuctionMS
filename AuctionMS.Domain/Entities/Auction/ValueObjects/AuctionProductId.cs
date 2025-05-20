
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionProductId
    {
        private AuctionProductId(Guid value) => Value = value;

        public static AuctionProductId Create()
        {
            return new AuctionProductId(Guid.NewGuid());
        }
        public static AuctionProductId? Create(Guid value)
        {
            //if (value == Guid.Empty) throw new NullAttributeException("Auction userId is required");

            return new AuctionProductId(value);
        }

        public static AuctionProductId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
