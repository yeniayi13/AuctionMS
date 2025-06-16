



namespace AuctionMS.Application.Saga.Events
{
    public class AuctionStartedEvent
    {
        public Guid AuctionId { get; set; }
    }

    public class BidPlacedEvent
    {
        public Guid AuctionId { get; set; }

        public BidPlacedEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class AuctionEndedEvent { }

    public class AuctionCanceledEvent
    {
        public Guid AuctionId { get; set; }

        public AuctionCanceledEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class PaymentReceivedEvent { }
}
