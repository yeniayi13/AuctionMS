using System.Threading.Tasks;

namespace AuctionMS.Core.Service.Auction
{
    public interface IPaymentService
    {
        Task<string?> GetPaymentIdByAuctionIdAsync(string auctionId);
        

    }
}
