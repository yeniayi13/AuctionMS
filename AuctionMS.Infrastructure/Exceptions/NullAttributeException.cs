namespace AuctionMS.Infrastructure.Exceptions
{


        public class NullAttributeException : Exception
        {
            public NullAttributeException() { }

            public NullAttributeException(string message)
                : base(message) { }

            public NullAttributeException(string message, Exception inner)
                : base(message, inner) { }
        }
    }