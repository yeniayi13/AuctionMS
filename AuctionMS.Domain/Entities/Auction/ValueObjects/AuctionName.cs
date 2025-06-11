//using AuctionMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionName
    {
        private const string Pattern = @"^[a-zA-Z]+$";
        private AuctionName(string value) => Value = value;

        public static AuctionName Create(string value)
        {
            try
            {
               // if (string.IsNullOrEmpty(value)) throw new NullAttributeException("Auction Name  is required");
               // if (!NameRegex().IsMatch(value)) throw new InvalidAttributeException("Auction Name is invalid");

                return new AuctionName(value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string Value { get; init; } //*init no permite setear desde afuera, solo desde el constructor
        [GeneratedRegex(Pattern)]
        private static partial Regex NameRegex();
    }
}
