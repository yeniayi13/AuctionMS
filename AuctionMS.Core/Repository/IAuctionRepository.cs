
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Core.Repository
{
    public interface IAuctionRepository
    {
        Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId, AuctionProductId productId);
        Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId, AuctionProductId productId);

        Task AddAsync(AuctionEntity auction);
        Task DeleteAsync(AuctionId id);
        Task<List<AuctionEntity>> GetAllAsync(AuctionUserId userId);
        Task<AuctionEntity?> UpdateAsync(AuctionEntity auction);
    }
}