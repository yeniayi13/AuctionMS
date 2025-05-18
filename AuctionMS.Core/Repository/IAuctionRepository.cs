
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Core.Repository
{
    public interface IAuctionRepository
    {
        Task<AuctionEntity?> GetByIdAsync(AuctionId id, AuctionUserId userId);
        Task<AuctionEntity?> GetByNameAsync(AuctionName name, AuctionUserId userId);
        Task<List<AuctionEntity?>> GetAvailableProductsAsync(AuctionUserId userId, decimal? priceBase = null, decimal? priceReserva = null);

        Task AddAsync(AuctionEntity auction);
        Task DeleteAsync(AuctionId id);
        Task<List<AuctionEntity>> GetAllAsync(AuctionUserId userId);
        Task<AuctionEntity?> UpdateAsync(AuctionEntity auction);
    }
}