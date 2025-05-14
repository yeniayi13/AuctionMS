using AuctionMS.Common.Exceptions;
using System.Text.RegularExpressions;

namespace AuctionMS.Domain.Entities.Auction.ValueObjects
{
    public partial class AuctionImage
    {
        private const string Pattern = @"^(http(s)?://)?([a-zA-Z0-9_-]+\.)+[a-zA-Z]+(/.*\.(jpg|jpeg|png|bmp|gif))$";

        private AuctionImage(string url) => Url = url;

        public static AuctionImage Create(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) throw new NullAttributeException("Image URL is required");
                if (!UrlRegex().IsMatch(url)) throw new InvalidAttributeException("Invalid image URL format");

                return new AuctionImage(url);
            }
            catch (Exception e)
            {
                throw; 
            }
        }

        public string Url { get; init; } // Inmutable, solo se asigna desde el constructor

        [GeneratedRegex(Pattern)]
        private static partial Regex UrlRegex();
    }
}