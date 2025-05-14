using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionMS.Common.Exceptions
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