using AuctionMS.Common.Dtos.Auction.Response;
using System.Threading.Tasks;

namespace AuctionMS.Core.Service.Auction
{
    public interface IPaymentService
    {
        Task<List<GetPaymentDto>> GetPaymentsByAuctionIdAsync(Guid auctionId);



    }
}
