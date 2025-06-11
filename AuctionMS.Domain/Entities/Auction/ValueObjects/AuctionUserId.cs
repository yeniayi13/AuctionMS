
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionUserId
    {
        private AuctionUserId(Guid value) => Value = value;

        public static AuctionUserId Create()
        {
            return new AuctionUserId(Guid.NewGuid());
        }
        public static AuctionUserId? Create(Guid value)
        {
            //if (value == Guid.Empty) throw new NullAttributeException("Auction userId is required");

            return new AuctionUserId(value);
        }

        public static AuctionUserId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
