using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionPaymentId
    {
        private AuctionPaymentId(Guid value) => Value = value;

    public static AuctionPaymentId Create()
    {
        return new AuctionPaymentId(Guid.NewGuid());
    }
    public static AuctionPaymentId? Create(Guid value)
    {
        //if (value == Guid.Empty) throw new NullAttributeException("Auction userId is required");

        return new AuctionPaymentId(value);
    }

    public static AuctionPaymentId Create(object value)
    {
        throw new NotImplementedException();
    }

    public Guid Value { get; init; }
}
}
