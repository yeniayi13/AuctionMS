using System.Diagnostics.CodeAnalysis;

namespace AuctionMS.Infrastructure.Settings
{
    public class AppSettings
    {
        [ExcludeFromCodeCoverage]
        public string? key1 { get; set; }
    }
}
