

namespace AuctionMS.Application.Saga.Events
{
    public class AuctionStartedEvent
    {
        public Guid AuctionId { get; set; }
    }

    public class BidPlacedEvent
    {
        public Guid AuctionId { get; set; }
        public Guid BidId { get; set; }
        public Guid UserId { get; set; }

        public BidPlacedEvent(Guid auctionId, Guid bidId, Guid userId)
        {
            AuctionId = auctionId;
            BidId = bidId;
            UserId = userId;
        }
    }

    public class AuctionEndedEvent
    {
        public Guid AuctionId { get; set; }

        public AuctionEndedEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class AuctionCanceledEvent
    {
        public Guid AuctionId { get; set; }

        public AuctionCanceledEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }

    public class PaymentReceivedEvent
    {
        public Guid AuctionId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }

        public PaymentReceivedEvent(Guid auctionId, Guid paymentId, Guid userId)
        {
            AuctionId = auctionId;
            PaymentId = paymentId;
            UserId = userId;
        }
    }
}





/*
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
*/