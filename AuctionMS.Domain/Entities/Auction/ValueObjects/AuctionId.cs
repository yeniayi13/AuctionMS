
namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public class AuctionId
    {
        private AuctionId(Guid value) => Value = value;

        public static AuctionId Create()
        {
            return new AuctionId(Guid.NewGuid());
        }
        public static AuctionId? Create(Guid value)
        {
            // if (value == Guid.Empty) throw new NullAttributeException("Auction id is required");

            return new AuctionId(value);
        }

        public static AuctionId Create(object value)
        {
            throw new NotImplementedException();
        }

        public Guid Value { get; init; }
    }
}
