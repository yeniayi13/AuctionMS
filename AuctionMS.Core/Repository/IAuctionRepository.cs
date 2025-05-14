
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Enum;

namespace AuctionMS.Core.Repository
{
    public interface IAuctionRepository
    {
        Task<AuctionEntity?> GetByIdAsync(AuctionId id/*, Expression<Func<Provider, object>> include*/);
        Task<List<AuctionEntity>> GetFilteredProductsAsync(AuctionPriceBase? priceBase, AuctionPriceReserva? priceReserva);
        Task<AuctionEntity?> GetByNameAsync(AuctionName name/*, Expression<Func<Provider, object>> include*/);
        Task AddAsync(AuctionEntity auction);
        Task DeleteAsync(AuctionId id);
        Task<List<AuctionEntity>> GetAllAsync();
        Task<AuctionEntity?> UpdateAsync(AuctionEntity auction);
    }
}