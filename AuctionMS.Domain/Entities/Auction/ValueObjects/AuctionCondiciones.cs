//using AuctionMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionCondiciones
    {
        private AuctionCondiciones(string value) => Value = value;

        public static AuctionCondiciones Create(string value)
        {
            try
            {
               // if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Auction condiciones is required");

                return new AuctionCondiciones(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor
    }
}