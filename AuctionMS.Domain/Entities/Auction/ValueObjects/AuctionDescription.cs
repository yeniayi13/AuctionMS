//using AuctionMS.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    [ExcludeFromCodeCoverage]
    public partial class AuctionDescription
    {
        private AuctionDescription(string value) => Value = value;

        public static AuctionDescription Create(string value)
        {
            try
            {
              //  if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Auction description is required");

                return new AuctionDescription(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor
    }
}