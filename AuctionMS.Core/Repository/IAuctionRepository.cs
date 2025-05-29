
using AuctionMS.Domain.Entities.Auction;
using AuctionMS.Domain.Entities.Auction.ValueObjects;
using AuctionMS.Common.Dtos.Auction.Response;

namespace AuctionMS.Core.Repository
{
    public interface IAuctionRepository
    {


        Task AddAsync(AuctionEntity auction);
        Task DeleteAsync(AuctionId id);
        Task<AuctionEntity?> UpdateAsync(AuctionEntity auction);


       


    }
}