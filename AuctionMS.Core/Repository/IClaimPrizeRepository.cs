using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Domain.Entities.Auction;

namespace AuctionMS.Core.Repository
{
    public interface IClaimPrizeRepository
    {
        Task AddAsync(ClaimPrizeAuction claim);
        Task<List<ClaimPrizeAuction>> GetByUserIdAsync(Guid userId);
        Task<ClaimPrizeAuction?> GetByAuctionIdAsync(Guid auctionId);
    }

}
