using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Domain.Entities.Auction;

namespace AuctionMS.Core.Repository
{
    public interface IAuctionRepositoryMongo
    {
        Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId, AuctionProductId productId);
        Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId, AuctionProductId productId);

        Task<List<AuctionEntity>> GetAllAsync(AuctionUserId userId);
    }
}
