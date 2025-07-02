


namespace AuctionMS.Application.Saga.Events
{
 
    public class AuctionCreatedEvent
    {
        public Guid AuctionId { get; set; }
       

        public AuctionCreatedEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }
    public class AuctionStartedEvent
    {
        public Guid AuctionId { get; set; }

        public AuctionStartedEvent(Guid auctionId)
        {
            AuctionId = auctionId;
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


    public class AuctionCompletedEvent
    {
        public Guid AuctionId { get; set; }


        public AuctionCompletedEvent(Guid auctionId)
        {
            AuctionId = auctionId;
        }
    }
}