//using AuctionMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionPriceBase
    {
        private const string Pattern = @"^\d+\.\d{2}$";
        private AuctionPriceBase (decimal value) => Value = value;
        //TODO REvisar si necesita el regex
        public static AuctionPriceBase Create(decimal value)
        {
            try
            {
               // if (value == default) throw new NullAttributeException("Value is required");
                //if (!BasePriceRegex().IsMatch(value)) throw new InvalidAttributeException("Client ci is invalid");
               // if (value < 0) throw new InvalidAttributeException("Value is invalid");

                return new AuctionPriceBase(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public decimal Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor

        [GeneratedRegex(Pattern)]
        private static partial Regex BasePriceRegex();
    }
}