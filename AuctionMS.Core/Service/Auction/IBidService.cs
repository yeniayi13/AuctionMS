using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMS.Core.Service.Auction
{
    // Interface para el servicio
    public interface IBidService
    {
        Task<bool> BidExists(Guid auctionBidId, Guid auctionId);
        Task<bool> IsWinningBid(Guid auctionBidId, Guid auctionId);
    }
}
