using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Infrastructure.Exceptions
{

    [ExcludeFromCodeCoverage]

    public class NullAttributeException : Exception
        {
            public NullAttributeException() { }

            public NullAttributeException(string message)
                : base(message) { }

            public NullAttributeException(string message, Exception inner)
                : base(message, inner) { }
        }
    }