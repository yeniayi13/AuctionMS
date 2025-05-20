namespace AuctionMS.Infrastructure.Exceptions
{
    public class AuctionNotFoundException : Exception
    {
        public AuctionNotFoundException() { }

        public AuctionNotFoundException(string message)
            : base(message) { }

        public AuctionNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}